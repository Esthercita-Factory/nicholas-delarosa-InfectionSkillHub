namespace InfectionSkillHub.Application.Dtos.Instructors;

public sealed record InstructorResponse(Guid Id, string FullName, string Email, string Specialty);
