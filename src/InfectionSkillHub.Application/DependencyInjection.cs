using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Services.Auth;
using InfectionSkillHub.Application.Services.Certificates;
using InfectionSkillHub.Application.Services.Enrollments;
using InfectionSkillHub.Application.Services.Instructors;
using InfectionSkillHub.Application.Services.Reports;
using InfectionSkillHub.Application.Services.Workshops;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfectionSkillHub.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<InfectionSkillHubOptions>(configuration.GetSection(InfectionSkillHubOptions.SectionName));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IInstructorService, InstructorService>();
        services.AddScoped<IWorkshopService, WorkshopService>();
        services.AddScoped<IEnrollmentRegistrar, EnrollmentRegistrar>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<IEnrollmentImportService, EnrollmentImportService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
