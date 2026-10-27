using System.ComponentModel.DataAnnotations;
using InfectionSkillHub.Domain.Enums;

namespace InfectionSkillHub.Application.Dtos.Enrollments;

public sealed class EnrollRequest
{
    [Required]
    public Guid? WorkshopId { get; init; }
}

public sealed record EnrollmentWorkshopSummary(
    Guid Id,
    string Code,
    string Title,
    string InstructorFullName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt);

/// <param name="CancellationDeadline">Last moment the participant can cancel by themselves (24 hours before the start).</param>
/// <param name="CanCancel">True while the participant can still cancel this enrollment.</param>
/// <param name="HasCertificate">Only active enrollments have a certificate.</param>
public sealed record EnrollmentResponse(
    Guid Id,
    EnrollmentStatus Status,
    DateTimeOffset EnrolledAt,
    DateTimeOffset? CancelledAt,
    string? CancelledBy,
    DateTimeOffset CancellationDeadline,
    bool CanCancel,
    bool HasCertificate,
    Guid ParticipantId,
    string ParticipantFullName,
    string ParticipantEmail,
    EnrollmentWorkshopSummary Workshop);

/// <param name="RowNumber">Row number inside the spreadsheet (the header is row 1).</param>
public sealed record RejectedRowResponse(int RowNumber, string ParticipantEmail, string WorkshopCode, string Reason);

public sealed record EnrollmentImportResponse(
    int TotalRows,
    int EnrolledCount,
    int RejectedCount,
    IReadOnlyList<RejectedRowResponse> RejectedRows);
