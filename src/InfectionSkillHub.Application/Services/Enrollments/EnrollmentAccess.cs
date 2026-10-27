using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Domain.Entities;

namespace InfectionSkillHub.Application.Services.Enrollments;

/// <summary>Employees reach any enrollment; a participant only their own.</summary>
internal static class EnrollmentAccess
{
    public static Error? Check(Enrollment? enrollment, Guid enrollmentId, RequestingUser user)
    {
        if (enrollment is null)
        {
            return Error.NotFound($"Enrollment {enrollmentId} does not exist.");
        }

        return user.IsEmployee || enrollment.Participant.UserId == user.UserId
            ? null
            : Error.Forbidden("You can only access your own enrollments.");
    }
}
