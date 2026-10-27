namespace InfectionSkillHub.Application.Dtos.Certificates;

/// <summary>Data printed on the enrollment certificate. Dates are already in the local time of the center.</summary>
public sealed record EnrollmentCertificate(
    string OrganizationName,
    Guid EnrollmentId,
    string ParticipantFullName,
    string ParticipantEmail,
    string WorkshopCode,
    string WorkshopTitle,
    string InstructorFullName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    DateTimeOffset EnrolledAt,
    DateTimeOffset GeneratedAt,
    string TimeZoneId);

public sealed record CertificateFile(byte[] Content, string FileName)
{
    public const string ContentType = "application/pdf";
}
