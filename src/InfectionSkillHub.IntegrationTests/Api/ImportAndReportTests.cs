using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClosedXML.Excel;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Application.Dtos.Reports;
using InfectionSkillHub.IntegrationTests.Common;

namespace InfectionSkillHub.IntegrationTests.Api;

public class ImportAndReportTests : ApiTestBase
{
    public ImportAndReportTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Import_SavesValidRowsAndReportsEveryRejectedRowWithItsNumber()
    {
        var workshop = await CreateWorkshopAsync(capacity: 2, startsIn: TimeSpan.FromDays(70));
        var (_, first) = await NewParticipantAsync();
        var (_, second) = await NewParticipantAsync();
        var (_, third) = await NewParticipantAsync();

        var file = Workbook(
            ["ParticipantEmail", "WorkshopCode"],
            [first.User.Email, workshop.Code],
            ["nobody@correo.com", workshop.Code],
            [second.User.Email, "XXX-999"],
            [first.User.Email, workshop.Code.ToLowerInvariant()],
            [second.User.Email, workshop.Code],
            [third.User.Email, workshop.Code]);

        var result = await ImportAsync(await EmployeeAsync(), file);

        Assert.Equal(6, result.TotalRows);
        Assert.Equal(2, result.EnrolledCount);
        Assert.Equal(4, result.RejectedCount);
        Assert.Equal([3, 4, 5, 7], result.RejectedRows.Select(r => r.RowNumber));
        Assert.Contains("nobody@correo.com", result.RejectedRows[0].Reason);
        Assert.Contains("XXX-999", result.RejectedRows[1].Reason);
        Assert.Contains("already has an active enrollment", result.RejectedRows[2].Reason);
        Assert.Contains("full", result.RejectedRows[3].Reason);
    }

    [Fact]
    public async Task Import_RejectsAFileWithoutTheRequiredColumns()
    {
        var file = Workbook(["Email", "Code"], ["a@correo.com", "FOT-001"]);

        var response = await PostFileAsync(await EmployeeAsync(), file);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Import_IsForEmployeesOnly()
    {
        var (participant, _) = await NewParticipantAsync();

        var response = await PostFileAsync(participant, Workbook(["ParticipantEmail", "WorkshopCode"]));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task TopWorkshops_AreOrderedByActiveEnrollments()
    {
        var busy = await CreateWorkshopAsync(capacity: 50, startsIn: TimeSpan.FromDays(71));
        for (var i = 0; i < 40; i++)
        {
            var (participant, _) = await NewParticipantAsync();
            Assert.Equal(HttpStatusCode.Created, (await EnrollAsync(participant, busy.Id)).StatusCode);
        }

        var report = await (await EmployeeAsync()).GetFromJsonAsync<List<TopWorkshopResponse>>("/api/v1/reports/top-workshops?limit=100", Json);

        Assert.Equal(busy.Id, report![0].WorkshopId);
        Assert.Equal(40, report[0].ActiveEnrollments);
        Assert.Equal(Enumerable.Range(1, report.Count), report.Select(r => r.Rank));
        Assert.True(report.Zip(report.Skip(1)).All(pair => pair.First.ActiveEnrollments >= pair.Second.ActiveEnrollments));
    }

    private async Task<EnrollmentImportResponse> ImportAsync(HttpClient employee, byte[] file)
    {
        var response = await PostFileAsync(employee, file);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<EnrollmentImportResponse>(Json))!;
    }

    private static Task<HttpResponseMessage> PostFileAsync(HttpClient client, byte[] file)
    {
        var content = new ByteArrayContent(file);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        return client.PostAsync("/api/v1/enrollments/import", new MultipartFormDataContent { { content, "file", "enrollments.xlsx" } });
    }

    private static byte[] Workbook(params string[][] rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Enrollments");
        for (var r = 0; r < rows.Length; r++)
        {
            for (var c = 0; c < rows[r].Length; c++)
            {
                sheet.Cell(r + 1, c + 1).Value = rows[r][c];
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
