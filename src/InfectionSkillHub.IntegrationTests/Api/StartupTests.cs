using System.Net;
using System.Net.Http.Json;
using InfectionSkillHub.Application.Dtos.Instructors;
using InfectionSkillHub.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace InfectionSkillHub.IntegrationTests.Api;

public class StartupTests : ApiTestBase
{
    public StartupTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Startup_AppliesTheMigrationsAndLoadsTheExcelData()
    {
        var pending = await QueryAsync(db => db.Database.GetPendingMigrationsAsync());
        Assert.Empty(pending);

        Assert.Equal(6, await QueryAsync(db => db.Instructors.CountAsync()));
        Assert.True(await QueryAsync(db => db.Participants.CountAsync()) >= 30);
        Assert.True(await QueryAsync(db => db.Workshops.CountAsync(w => w.Code.StartsWith("FOT-"))) == 3);
    }

    [Fact]
    public async Task Database_HasThePartialUniqueIndexForActiveEnrollments()
    {
        var definition = await QueryAsync(db => db.Database
            .SqlQuery<string>($"SELECT indexdef AS \"Value\" FROM pg_indexes WHERE indexname = 'ux_enrollments_active_participant_workshop'")
            .SingleAsync());

        Assert.Contains("UNIQUE", definition);
        Assert.Contains("WHERE", definition);
        Assert.Contains("'Active'", definition);
    }

    [Fact]
    public async Task Health_ReportsTheDatabaseAsReachable()
    {
        var response = await Anonymous().GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SeededEmployee_CanSignInAndListTheInstructors()
    {
        var instructors = await (await EmployeeAsync()).GetFromJsonAsync<List<InstructorResponse>>("/api/v1/instructors", Json);

        Assert.Equal(6, instructors!.Count);
        Assert.Contains(instructors, i => i.Email == EmployeeEmail);
    }
}
