using InfectionSkillHub.Application.Common;

namespace InfectionSkillHub.Application.Abstractions;

/// <param name="RowNumber">Row number inside the spreadsheet (the header is row 1).</param>
public sealed record EnrollmentSheetRow(int RowNumber, string ParticipantEmail, string WorkshopCode);

/// <summary>Reads the rows of a bulk enrollment spreadsheet (ParticipantEmail and WorkshopCode columns).</summary>
public interface IEnrollmentSheetReader
{
    /// <summary>Fails when the file cannot be read or the required columns are missing. Empty rows are skipped.</summary>
    Result<IReadOnlyList<EnrollmentSheetRow>> Read(Stream content);
}
