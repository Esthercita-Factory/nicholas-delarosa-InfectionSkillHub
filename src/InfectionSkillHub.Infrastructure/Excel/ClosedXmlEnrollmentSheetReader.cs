using ClosedXML.Excel;
using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;

namespace InfectionSkillHub.Infrastructure.Excel;

public sealed class ClosedXmlEnrollmentSheetReader : IEnrollmentSheetReader
{
    public const string PreferredSheet = "Enrollments";
    public const string ParticipantEmailColumn = "ParticipantEmail";
    public const string WorkshopCodeColumn = "WorkshopCode";

    public Result<IReadOnlyList<EnrollmentSheetRow>> Read(Stream content)
    {
        XLWorkbook workbook;
        try
        {
            workbook = new XLWorkbook(content);
        }
        catch (Exception)
        {
            return Error.Validation("The file is not a valid Excel workbook (.xlsx).", "File");
        }

        using (workbook)
        {
            // The InfectionSkillHub data file brings several sheets; a file with a single sheet is read as it is.
            var sheet = workbook.Worksheets.FirstOrDefault(s => s.Name.Equals(PreferredSheet, StringComparison.OrdinalIgnoreCase))
                ?? workbook.Worksheets.First();

            var table = SheetTable.From(sheet);
            var missing = table.MissingColumns(ParticipantEmailColumn, WorkshopCodeColumn);
            if (missing.Count > 0)
            {
                return Error.Validation(
                    $"Sheet '{sheet.Name}' is missing the column(s): {string.Join(", ", missing)}. Expected {ParticipantEmailColumn} and {WorkshopCodeColumn} in the first row.",
                    "File");
            }

            return Result.Success<IReadOnlyList<EnrollmentSheetRow>>(table.Rows()
                .Select(row => new EnrollmentSheetRow(row.Number, row.Text(ParticipantEmailColumn), row.Text(WorkshopCodeColumn)))
                .ToList());
        }
    }
}
