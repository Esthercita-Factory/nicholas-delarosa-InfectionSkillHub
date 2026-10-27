namespace InfectionSkillHub.Application.Common;

/// <summary>"InfectionSkillHub" configuration section.</summary>
public sealed class InfectionSkillHubOptions
{
    public const string SectionName = "InfectionSkillHub";

    public string OrganizationName { get; set; } = "InfectionSkillHub";

    /// <summary>
    /// Local time zone of the center (IANA id). Dates are stored in UTC; this zone is used to read
    /// the dates of the initial data and to print them on the enrollment certificate.
    /// </summary>
    public string TimeZoneId { get; set; } = "America/Bogota";

    public TimeZoneInfo TimeZone => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

    public DateTimeOffset ToLocal(DateTimeOffset value) => TimeZoneInfo.ConvertTime(value, TimeZone);
}
