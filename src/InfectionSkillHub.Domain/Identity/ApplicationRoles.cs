namespace InfectionSkillHub.Domain.Identity;

public static class ApplicationRoles
{
    /// <summary>InfectionSkillHub staff. Every employee is an instructor: publishes workshops and manages any enrollment.</summary>
    public const string Employee = "Employee";

    /// <summary>Anyone who signs up on the platform. Only manages their own enrollments.</summary>
    public const string Participant = "Participant";
}
