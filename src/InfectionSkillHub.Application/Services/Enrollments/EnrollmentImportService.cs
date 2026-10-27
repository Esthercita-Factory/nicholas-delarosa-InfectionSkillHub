using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InfectionSkillHub.Application.Services.Enrollments;

public sealed class EnrollmentImportService : IEnrollmentImportService
{
    public const int MaxRows = 5000;

    private readonly IEnrollmentSheetReader _reader;
    private readonly IEnrollmentRegistrar _registrar;
    private readonly IParticipantRepository _participants;
    private readonly IWorkshopRepository _workshops;
    private readonly ILogger<EnrollmentImportService> _logger;

    public EnrollmentImportService(
        IEnrollmentSheetReader reader,
        IEnrollmentRegistrar registrar,
        IParticipantRepository participants,
        IWorkshopRepository workshops,
        ILogger<EnrollmentImportService> logger)
    {
        _reader = reader;
        _registrar = registrar;
        _participants = participants;
        _workshops = workshops;
        _logger = logger;
    }

    public async Task<Result<EnrollmentImportResponse>> ImportAsync(Stream content, CancellationToken cancellationToken = default)
    {
        var read = _reader.Read(content);
        if (read.IsFailure)
        {
            return read.Error!;
        }

        var rows = read.Value!;
        if (rows.Count == 0)
        {
            return Error.Validation("The file has no rows to import.", "File");
        }

        if (rows.Count > MaxRows)
        {
            return Error.Validation($"The file has {rows.Count} rows; the maximum per import is {MaxRows}.", "File");
        }

        var rejected = new List<RejectedRowResponse>();
        var enrolled = 0;

        // Rows go one by one and in order: an earlier row of the same file counts for the
        // capacity, duplicate and overlap checks of the following ones.
        foreach (var row in rows)
        {
            var reason = await EnrollRowAsync(row, cancellationToken);
            if (reason is null)
            {
                enrolled++;
            }
            else
            {
                rejected.Add(new RejectedRowResponse(row.RowNumber, row.ParticipantEmail, row.WorkshopCode, reason));
            }
        }

        _logger.LogInformation("Enrollment import finished: {Enrolled} enrolled, {Rejected} rejected", enrolled, rejected.Count);

        return new EnrollmentImportResponse(rows.Count, enrolled, rejected.Count, rejected);
    }

    /// <returns>null when the row was enrolled; otherwise the rejection reason.</returns>
    private async Task<string?> EnrollRowAsync(EnrollmentSheetRow row, CancellationToken cancellationToken)
    {
        var email = row.ParticipantEmail.Trim().ToLowerInvariant();
        var code = Workshop.NormalizeCode(row.WorkshopCode);

        if (email.Length == 0)
        {
            return "ParticipantEmail is empty.";
        }

        if (code.Length == 0)
        {
            return "WorkshopCode is empty.";
        }

        var participant = await _participants.FindByEmailAsync(email, cancellationToken);
        if (participant is null)
        {
            return $"There is no participant registered with the email {email}.";
        }

        var workshop = await _workshops.FindByCodeAsync(code, cancellationToken);
        if (workshop is null)
        {
            return $"Workshop {code} does not exist.";
        }

        var result = await _registrar.EnrollAsync(participant, workshop, cancellationToken);
        return result.IsSuccess ? null : result.Error!.Message;
    }
}
