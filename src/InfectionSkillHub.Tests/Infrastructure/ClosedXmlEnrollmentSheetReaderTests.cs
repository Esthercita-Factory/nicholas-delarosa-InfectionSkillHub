using ClosedXML.Excel;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Infrastructure.Excel;
using InfectionSkillHub.Infrastructure.Persistence.Seed;

namespace InfectionSkillHub.Tests.Infrastructure;

public class ClosedXmlEnrollmentSheetReaderTests
{
    private readonly ClosedXmlEnrollmentSheetReader _reader = new();

    [Fact]
    public void Read_TakesTheEnrollmentsSheetOfTheInfectionSkillHubDataFile()
    {
        using var file = typeof(InfectionSkillHubDataSeeder).Assembly.GetManifestResourceStream(InfectionSkillHubDataSeeder.ResourceName)!;

        var result = _reader.Read(file);

        var rows = result.Value!;
        Assert.Equal(70, rows.Count);
        Assert.Equal(new(2, "daniela.morales@correo.com", "COC-001"), rows[0]);
    }

    [Fact]
    public void Read_UsesTheFirstSheetAndSkipsEmptyRows()
    {
        using var file = Workbook("Sheet1", ["ParticipantEmail", "WorkshopCode"], ["a@correo.com", "FOT-001"], ["", ""], [" b@correo.com ", "COC-001"]);

        var rows = _reader.Read(file).Value!;

        Assert.Collection(rows,
            row => Assert.Equal(new(2, "a@correo.com", "FOT-001"), row),
            row => Assert.Equal(new(4, "b@correo.com", "COC-001"), row));
    }

    [Fact]
    public void Read_FailsWhenAColumnIsMissing()
    {
        using var file = Workbook("Sheet1", ["Email", "WorkshopCode"], ["a@correo.com", "FOT-001"]);

        var result = _reader.Read(file);

        Assert.Equal(ErrorCode.Validation, result.Error!.Code);
        Assert.Contains("ParticipantEmail", result.Error.Message);
    }

    [Fact]
    public void Read_FailsForAFileThatIsNotAWorkbook()
    {
        using var file = new MemoryStream("not an excel file"u8.ToArray());

        Assert.Equal(ErrorCode.Validation, _reader.Read(file).Error!.Code);
    }

    private static MemoryStream Workbook(string sheetName, params string[][] rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(sheetName);
        for (var r = 0; r < rows.Length; r++)
        {
            for (var c = 0; c < rows[r].Length; c++)
            {
                sheet.Cell(r + 1, c + 1).Value = rows[r][c];
            }
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }
}
