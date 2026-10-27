using System.ComponentModel.DataAnnotations;
using InfectionSkillHub.Application.Dtos.Instructors;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Enums;

namespace InfectionSkillHub.Application.Dtos.Workshops;

public sealed class CreateWorkshopRequest
{
    /// <summary>Unique code, stored in uppercase.</summary>
    /// <example>FOT-004</example>
    [Required]
    [StringLength(Workshop.CodeMaxLength)]
    public string Code { get; init; } = string.Empty;

    /// <example>Fotografía nocturna</example>
    [Required]
    [StringLength(Workshop.TitleMaxLength)]
    public string Title { get; init; } = string.Empty;

    /// <example>Exposición larga, trípode y edición de cielos nocturnos</example>
    [Required]
    [StringLength(Workshop.DescriptionMaxLength)]
    public string Description { get; init; } = string.Empty;

    /// <summary>Email of an instructor already registered on the platform.</summary>
    /// <example>andres.mejia@correo.com</example>
    [Required]
    [EmailAddress]
    public string InstructorEmail { get; init; } = string.Empty;

    /// <summary>Start date and time with its UTC offset. Must be in the future.</summary>
    /// <example>2027-03-06T09:00:00-05:00</example>
    [Required]
    public DateTimeOffset? StartsAt { get; init; }

    /// <summary>End date and time with its UTC offset. Must be later than the start.</summary>
    /// <example>2027-03-06T12:00:00-05:00</example>
    [Required]
    public DateTimeOffset? EndsAt { get; init; }

    /// <example>15</example>
    [Required]
    [Range(1, Workshop.MaxCapacity)]
    public int? Capacity { get; init; }
}

/// <param name="AvailableSeats">Capacity minus active enrollments, calculated by the system.</param>
public sealed record WorkshopResponse(
    Guid Id,
    string Code,
    string Title,
    string Description,
    InstructorResponse Instructor,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int Capacity,
    int ActiveEnrollments,
    int AvailableSeats,
    bool HasStarted);

/// <summary>Enrollment of a workshop as the employees see it.</summary>
public sealed record WorkshopEnrollmentResponse(
    Guid EnrollmentId,
    Guid ParticipantId,
    string ParticipantFullName,
    string ParticipantEmail,
    EnrollmentStatus Status,
    DateTimeOffset EnrolledAt,
    DateTimeOffset? CancelledAt,
    string? CancelledBy);
