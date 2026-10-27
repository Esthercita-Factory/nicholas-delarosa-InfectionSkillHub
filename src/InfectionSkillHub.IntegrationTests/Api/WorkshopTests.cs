using System.Net;
using System.Net.Http.Json;
using InfectionSkillHub.Application.Dtos.Workshops;
using InfectionSkillHub.IntegrationTests.Common;

namespace InfectionSkillHub.IntegrationTests.Api;

public class WorkshopTests : ApiTestBase
{
    public WorkshopTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreatedWorkshop_AppearsInThePublicCatalogWithAllSeats()
    {
        var workshop = await CreateWorkshopAsync(capacity: 7, startsIn: TimeSpan.FromDays(41));

        var catalog = await Anonymous().GetFromJsonAsync<List<WorkshopResponse>>("/api/v1/workshops", Json);

        var listed = Assert.Single(catalog!, w => w.Id == workshop.Id);
        Assert.Equal(7, listed.AvailableSeats);
        Assert.False(listed.HasStarted);
    }

    [Fact]
    public async Task Create_RejectsADuplicatedCodeInAnyCase()
    {
        var workshop = await CreateWorkshopAsync(capacity: 5, startsIn: TimeSpan.FromDays(42));
        var start = DateTimeOffset.UtcNow.AddDays(43);

        var response = await (await EmployeeAsync()).PostAsJsonAsync("/api/v1/workshops", new
        {
            code = workshop.Code.ToLowerInvariant(),
            title = "Duplicate",
            description = "Duplicate",
            instructorEmail = EmployeeEmail,
            startsAt = start,
            endsAt = start.AddHours(2),
            capacity = 5
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsAPastStart()
    {
        var start = DateTimeOffset.UtcNow.AddHours(-1);

        var response = await (await EmployeeAsync()).PostAsJsonAsync("/api/v1/workshops", new
        {
            code = UniqueCode(),
            title = "Past",
            description = "Past",
            instructorEmail = EmployeeEmail,
            startsAt = start,
            endsAt = start.AddHours(2),
            capacity = 5
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Create_RejectsAnUnknownInstructor()
    {
        var start = DateTimeOffset.UtcNow.AddDays(44);

        var response = await (await EmployeeAsync()).PostAsJsonAsync("/api/v1/workshops", new
        {
            code = UniqueCode(),
            title = "Nobody",
            description = "Nobody",
            instructorEmail = "nobody@correo.com",
            startsAt = start,
            endsAt = start.AddHours(2),
            capacity = 5
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
