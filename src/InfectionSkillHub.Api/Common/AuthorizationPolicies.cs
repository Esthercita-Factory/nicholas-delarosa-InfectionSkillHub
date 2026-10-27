namespace InfectionSkillHub.Api.Common;

public static class AuthorizationPolicies
{
    /// <summary>InfectionSkillHub staff: publish workshops, see any enrollment, import and reports.</summary>
    public const string EmployeeOnly = "EmployeeOnly";

    /// <summary>Participants: enroll and manage their own enrollments.</summary>
    public const string ParticipantOnly = "ParticipantOnly";
}
