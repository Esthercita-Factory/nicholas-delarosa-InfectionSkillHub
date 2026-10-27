using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Infrastructure.Documents;
using InfectionSkillHub.Infrastructure.Excel;
using InfectionSkillHub.Infrastructure.Identity;
using InfectionSkillHub.Infrastructure.Persistence;
using InfectionSkillHub.Infrastructure.Persistence.Seed;
using InfectionSkillHub.Infrastructure.Repositories;
using InfectionSkillHub.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfectionSkillHub.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>Persistence, repositories, Identity, JWT issuing, PDF generation and Excel reading.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection is missing (set it in appsettings.Development.json or as an environment variable).");
        }

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<IWorkshopRepository, WorkshopRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

        services.AddScoped<IUserService, IdentityUserService>();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddSingleton<ICertificateGenerator, QuestPdfCertificateGenerator>();
        services.AddSingleton<IEnrollmentSheetReader, ClosedXmlEnrollmentSheetReader>();

        services.AddScoped<InfectionSkillHubDataSeeder>();

        return services;
    }
}
