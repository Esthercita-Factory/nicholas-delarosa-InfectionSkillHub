using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Application.Dtos.Instructors;
using InfectionSkillHub.Application.Dtos.Reports;
using InfectionSkillHub.Application.Dtos.Workshops;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Services;

namespace InfectionSkillHub.Application.Mappings;

/// <summary>Entity to DTO conversions. Entities never leave the Application layer.</summary>
public static class DtoMappings
{
    public static InstructorResponse ToResponse(this Instructor instructor)
        => new(instructor.Id, instructor.FullName, instructor.Email, instructor.Specialty);

    public static WorkshopResponse ToResponse(this WorkshopSeats seats, DateTimeOffset now)
    {
        var workshop = seats.Workshop;

        return new WorkshopResponse(
            workshop.Id,
            workshop.Code,
            workshop.Title,
            workshop.Description,
            workshop.Instructor.ToResponse(),
            workshop.StartsAt,
            workshop.EndsAt,
            workshop.Capacity,
            seats.ActiveEnrollments,
            EnrollmentPolicy.AvailableSeats(workshop, seats.ActiveEnrollments),
            workshop.HasStarted(now));
    }

    public static WorkshopEnrollmentResponse ToWorkshopEnrollmentResponse(this Enrollment enrollment)
        => new(
            enrollment.Id,
            enrollment.ParticipantId,
            enrollment.Participant.FullName,
            enrollment.Participant.Email,
            enrollment.Status,
            enrollment.EnrolledAt,
            enrollment.CancelledAt,
            enrollment.CancelledByEmail);

    public static EnrollmentResponse ToResponse(this Enrollment enrollment, DateTimeOffset now)
    {
        var workshop = enrollment.Workshop;

        return new EnrollmentResponse(
            enrollment.Id,
            enrollment.Status,
            enrollment.EnrolledAt,
            enrollment.CancelledAt,
            enrollment.CancelledByEmail,
            workshop.CancellationDeadline,
            enrollment.CanBeCancelledByParticipant(now),
            enrollment.IsActive,
            enrollment.ParticipantId,
            enrollment.Participant.FullName,
            enrollment.Participant.Email,
            new EnrollmentWorkshopSummary(
                workshop.Id,
                workshop.Code,
                workshop.Title,
                workshop.Instructor.FullName,
                workshop.StartsAt,
                workshop.EndsAt));
    }

    public static TopWorkshopResponse ToTopWorkshopResponse(this WorkshopSeats seats, int rank)
        => new(
            rank,
            seats.Workshop.Id,
            seats.Workshop.Code,
            seats.Workshop.Title,
            seats.Workshop.Instructor.FullName,
            seats.Workshop.StartsAt,
            seats.Workshop.Capacity,
            seats.ActiveEnrollments,
            EnrollmentPolicy.AvailableSeats(seats.Workshop, seats.ActiveEnrollments));
}
