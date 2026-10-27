using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Certificates;
using InfectionSkillHub.Application.Services.Enrollments;
using Microsoft.Extensions.Options;

namespace InfectionSkillHub.Application.Services.Certificates;

public sealed class CertificateService : ICertificateService
{
    private readonly IEnrollmentRepository _enrollments;
    private readonly ICertificateGenerator _generator;
    private readonly IClock _clock;
    private readonly InfectionSkillHubOptions _options;

    public CertificateService(
        IEnrollmentRepository enrollments,
        ICertificateGenerator generator,
        IClock clock,
        IOptions<InfectionSkillHubOptions> options)
    {
        _enrollments = enrollments;
        _generator = generator;
        _clock = clock;
        _options = options.Value;
    }

    public async Task<Result<CertificateFile>> GetAsync(Guid enrollmentId, RequestingUser user, CancellationToken cancellationToken = default)
    {
        var enrollment = await _enrollments.FindByIdAsync(enrollmentId, cancellationToken);
        if (EnrollmentAccess.Check(enrollment, enrollmentId, user) is { } denied)
        {
            return denied;
        }

        if (!enrollment!.IsActive)
        {
            return Error.BusinessRule("Cancelled enrollments have no certificate.");
        }

        var workshop = enrollment.Workshop;
        var certificate = new EnrollmentCertificate(
            _options.OrganizationName,
            enrollment.Id,
            enrollment.Participant.FullName,
            enrollment.Participant.Email,
            workshop.Code,
            workshop.Title,
            workshop.Instructor.FullName,
            _options.ToLocal(workshop.StartsAt),
            _options.ToLocal(workshop.EndsAt),
            _options.ToLocal(enrollment.EnrolledAt),
            _options.ToLocal(_clock.UtcNow),
            _options.TimeZoneId);

        var content = _generator.Generate(certificate);
        return new CertificateFile(content, $"certificate-{workshop.Code}-{enrollment.Id.ToString("N")[..8]}.pdf");
    }
}
