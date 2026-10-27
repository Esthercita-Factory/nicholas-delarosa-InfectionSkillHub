using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Services;

namespace InfectionSkillHub.Tests.Common;

/// <summary>Builders with valid defaults so each test only states what matters to it.</summary>
public static class TestData
{
    public static readonly DateTimeOffset Now = new(2026, 11, 1, 15, 0, 0, TimeSpan.Zero);

    public static Instructor Instructor(string email = "andres.mejia@correo.com")
        => global::InfectionSkillHub.Domain.Entities.Instructor.Create(Guid.NewGuid().ToString(), "Andrés Felipe Mejía Arango", email, "Fotografía");

    public static Participant Participant(string email = "jorge.perez@correo.com", string? userId = null)
        => global::InfectionSkillHub.Domain.Entities.Participant.Create(userId ?? Guid.NewGuid().ToString(), "Jorge Andrés Pérez Rojas", email, Now);

    /// <param name="startsIn">Time from <see cref="Now"/> to the start.</param>
    public static Workshop Workshop(
        string code = "FOT-001",
        TimeSpan? startsIn = null,
        TimeSpan? duration = null,
        int capacity = 10)
    {
        var startsAt = Now + (startsIn ?? TimeSpan.FromDays(10));
        return global::InfectionSkillHub.Domain.Entities.Workshop.Schedule(
            code,
            "Fotografía con celular",
            "Composición, luz y edición básica",
            Instructor(),
            startsAt,
            startsAt + (duration ?? TimeSpan.FromHours(4)),
            capacity,
            Now);
    }

    public static Enrollment Enrollment(Participant? participant = null, Workshop? workshop = null)
        => EnrollmentPolicy.Enroll(participant ?? Participant(), workshop ?? Workshop(), 0, [], Now);

    public static RequestingUser ParticipantUser(Participant participant)
        => new(participant.UserId, participant.Email, false);

    public static RequestingUser EmployeeUser() => new(Guid.NewGuid().ToString(), "rosa.barrios@correo.com", true);
}

public sealed class FakeClock : IClock
{
    public FakeClock(DateTimeOffset? now = null)
    {
        UtcNow = now ?? TestData.Now;
    }

    public DateTimeOffset UtcNow { get; set; }
}
