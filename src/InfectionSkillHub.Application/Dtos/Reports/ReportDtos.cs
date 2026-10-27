namespace InfectionSkillHub.Application.Dtos.Reports;

public sealed record TopWorkshopResponse(
    int Rank,
    Guid WorkshopId,
    string Code,
    string Title,
    string InstructorFullName,
    DateTimeOffset StartsAt,
    int Capacity,
    int ActiveEnrollments,
    int AvailableSeats);
