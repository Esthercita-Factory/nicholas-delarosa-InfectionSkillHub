using System.ComponentModel.DataAnnotations;

namespace InfectionSkillHub.Api.Requests;

public sealed class EnrollmentImportForm
{
    public const long MaxFileBytes = 5 * 1024 * 1024;

    /// <summary>
    /// .xlsx workbook with the columns ParticipantEmail and WorkshopCode in the first row. If it has a
    /// sheet named "Enrollments" that one is read (InfectionSkillHub_Data.xlsx works as it is); otherwise the first sheet.
    /// </summary>
    [Required]
    public IFormFile? File { get; init; }
}
