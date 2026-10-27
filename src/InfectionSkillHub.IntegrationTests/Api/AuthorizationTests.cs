using System.Net;
using System.Net.Http.Json;
using InfectionSkillHub.IntegrationTests.Common;

namespace InfectionSkillHub.IntegrationTests.Api;

public class AuthorizationTests : ApiTestBase
{
    public AuthorizationTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Catalog_IsPublic()
    {
        var response = await Anonymous().GetAsync("/api/v1/workshops");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("GET", "/api/v1/enrollments/mine")]
    [InlineData("GET", "/api/v1/reports/top-workshops")]
    [InlineData("GET", "/api/v1/instructors")]
    [InlineData("POST", "/api/v1/workshops")]
    [InlineData("POST", "/api/v1/enrollments")]
    public async Task ProtectedEndpoints_RequireAToken(string method, string path)
    {
        var response = await Anonymous().SendAsync(new HttpRequestMessage(new HttpMethod(method), path)
        {
            Content = method == "POST" ? JsonContent.Create(new { }) : null
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Participant_CannotUseEmployeeEndpoints()
    {
        var (participant, _) = await NewParticipantAsync();

        Assert.Equal(HttpStatusCode.Forbidden, (await participant.GetAsync("/api/v1/reports/top-workshops")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await participant.GetAsync("/api/v1/instructors")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await participant.PostAsJsonAsync("/api/v1/workshops", new { })).StatusCode);
    }

    [Fact]
    public async Task Employee_CannotEnroll()
    {
        var workshop = await CreateWorkshopAsync(capacity: 5, startsIn: TimeSpan.FromDays(40));

        var response = await EnrollAsync(await EmployeeAsync(), workshop.Id);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Register_RejectsADuplicatedEmail()
    {
        var (_, session) = await NewParticipantAsync();

        var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/register", new
        {
            fullName = "Same Email",
            email = session.User.Email,
            password = ParticipantPassword
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_RejectsAWrongPassword()
    {
        var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/login", new { email = EmployeeEmail, password = "Wrong2026*" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
