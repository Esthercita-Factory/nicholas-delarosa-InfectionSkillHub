using InfectionSkillHub.Application.Dtos.Certificates;

namespace InfectionSkillHub.Application.Abstractions;

/// <summary>Renders the enrollment certificate as a PDF document.</summary>
public interface ICertificateGenerator
{
    byte[] Generate(EnrollmentCertificate certificate);
}
