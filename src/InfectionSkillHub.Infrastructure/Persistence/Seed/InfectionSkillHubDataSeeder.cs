using System.Globalization;
using ClosedXML.Excel;
using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Identity;
using InfectionSkillHub.Infrastructure.Excel;
using InfectionSkillHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InfectionSkillHub.Infrastructure.Persistence.Seed;

/// <summary>
/// Loads the Instructors, Participants and Workshops sheets of InfectionSkillHub_Data.xlsx (embedded in
/// this assembly). It is idempotent: whatever already exists, matched by email or code, is skipped.
/// The Enrollments sheet is never loaded here; it goes through POST /api/v1/enrollments/import.
/// </summary>
public sealed class InfectionSkillHubDataSeeder
{
    public const string ResourceName = "InfectionSkillHub.Infrastructure.Seed.InfectionSkillHub_Data.xlsx";

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IClock _clock;
    private readonly InfectionSkillHubOptions _options;
    private readonly ILogger<InfectionSkillHubDataSeeder> _logger;

    public InfectionSkillHubDataSeeder(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IClock clock,
        IOptions<InfectionSkillHubOptions> options,
        ILogger<InfectionSkillHubDataSeeder> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _clock = clock;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureRolesAsync();

        await using var stream = typeof(InfectionSkillHubDataSeeder).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded resource {ResourceName} was not found.");
        using var workbook = new XLWorkbook(stream);

        var instructors = await SeedInstructorsAsync(Table(workbook, "Instructors", "FullName", "Email", "Password", "Specialty"), cancellationToken);
        var participants = await SeedParticipantsAsync(Table(workbook, "Participants", "FullName", "Email", "Password"), cancellationToken);
        var workshops = await SeedWorkshopsAsync(
            Table(workbook, "Workshops", "Code", "Title", "Description", "InstructorEmail", "StartDateTime", "EndDateTime", "Capacity"),
            cancellationToken);

        _logger.LogInformation(
            "Initial data loaded: {Instructors} instructors, {Participants} participants and {Workshops} workshops added",
            instructors, participants, workshops);
    }

    private async Task EnsureRolesAsync()
    {
        foreach (var role in new[] { ApplicationRoles.Employee, ApplicationRoles.Participant })
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                EnsureSucceeded(await _roleManager.CreateAsync(new IdentityRole(role)), $"Could not create the role {role}.");
            }
        }
    }

    private async Task<int> SeedInstructorsAsync(SheetTable table, CancellationToken cancellationToken)
    {
        var added = 0;

        foreach (var row in table.Rows())
        {
            var email = row.Text("Email").ToLowerInvariant();
            if (await _db.Instructors.AnyAsync(i => i.Email == email, cancellationToken))
            {
                continue;
            }

            var user = await EnsureUserAsync(email, row.Text("FullName"), row.Text("Password"), ApplicationRoles.Employee);
            _db.Instructors.Add(Instructor.Create(user.Id, row.Text("FullName"), email, row.Text("Specialty")));
            await _db.SaveChangesAsync(cancellationToken);
            added++;
        }

        return added;
    }

    private async Task<int> SeedParticipantsAsync(SheetTable table, CancellationToken cancellationToken)
    {
        var added = 0;

        foreach (var row in table.Rows())
        {
            var email = row.Text("Email").ToLowerInvariant();
            if (await _db.Participants.AnyAsync(p => p.Email == email, cancellationToken))
            {
                continue;
            }

            var user = await EnsureUserAsync(email, row.Text("FullName"), row.Text("Password"), ApplicationRoles.Participant);
            _db.Participants.Add(Participant.Create(user.Id, row.Text("FullName"), email, _clock.UtcNow));
            await _db.SaveChangesAsync(cancellationToken);
            added++;
        }

        return added;
    }

    private async Task<int> SeedWorkshopsAsync(SheetTable table, CancellationToken cancellationToken)
    {
        var added = 0;
        var zone = _options.TimeZone;

        foreach (var row in table.Rows())
        {
            var code = Workshop.NormalizeCode(row.Text("Code"));
            if (await _db.Workshops.AnyAsync(w => w.Code == code, cancellationToken))
            {
                continue;
            }

            var instructorEmail = row.Text("InstructorEmail").ToLowerInvariant();
            var instructor = await _db.Instructors.FirstOrDefaultAsync(i => i.Email == instructorEmail, cancellationToken);
            if (instructor is null)
            {
                _logger.LogWarning("Workshop {Code} skipped: instructor {Email} does not exist", code, instructorEmail);
                continue;
            }

            _db.Workshops.Add(Workshop.Restore(
                code,
                row.Text("Title"),
                row.Text("Description"),
                instructor,
                LocalDate(row.Cell("StartDateTime"), zone),
                LocalDate(row.Cell("EndDateTime"), zone),
                (int)row.Cell("Capacity").GetDouble(),
                _clock.UtcNow));
            added++;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return added;
    }

    private async Task<ApplicationUser> EnsureUserAsync(string email, string fullName, string password, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName.Trim()
            };
            EnsureSucceeded(await _userManager.CreateAsync(user, password), $"Could not create the account {email}.");
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            EnsureSucceeded(await _userManager.AddToRoleAsync(user, role), $"Could not assign the role {role} to {email}.");
        }

        return user;
    }

    private static SheetTable Table(XLWorkbook workbook, string sheetName, params string[] columns)
    {
        if (!workbook.TryGetWorksheet(sheetName, out var sheet))
        {
            throw new InvalidOperationException($"The initial data file has no sheet named {sheetName}.");
        }

        var table = SheetTable.From(sheet);
        var missing = table.MissingColumns(columns);
        if (missing.Count > 0)
        {
            throw new InvalidOperationException($"Sheet {sheetName} is missing the column(s): {string.Join(", ", missing)}.");
        }

        return table;
    }

    /// <summary>The spreadsheet holds local times of the center; they are converted to UTC.</summary>
    private static DateTimeOffset LocalDate(IXLCell cell, TimeZoneInfo zone)
    {
        var value = cell.DataType switch
        {
            XLDataType.DateTime => cell.GetDateTime(),
            XLDataType.Number => DateTime.FromOADate(cell.GetDouble()),
            _ => DateTime.Parse(cell.GetFormattedString(), CultureInfo.InvariantCulture)
        };

        // Excel stores dates as fractions of a day; rounding to the minute removes the floating point noise.
        var local = DateTime.SpecifyKind(
            new DateTime((value.Ticks + TimeSpan.TicksPerMinute / 2) / TimeSpan.TicksPerMinute * TimeSpan.TicksPerMinute),
            DateTimeKind.Unspecified);

        return new DateTimeOffset(local, zone.GetUtcOffset(local)).ToUniversalTime();
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"{message} {string.Join(" ", result.Errors.Select(e => e.Description))}");
        }
    }
}
