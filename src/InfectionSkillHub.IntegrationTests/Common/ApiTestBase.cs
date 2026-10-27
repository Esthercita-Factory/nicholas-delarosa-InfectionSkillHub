using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using InfectionSkillHub.Application.Dtos.Auth;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Application.Dtos.Workshops;
using InfectionSkillHub.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace InfectionSkillHub.IntegrationTests.Common;

/// <summary>Helpers that drive the API the way a client does: HTTP, JSON and a bearer token.</summary>
[Collection(ApiCollection.Name)]
public abstract class ApiTestBase
{
    /// <summary>Instructor (employee) loaded by the seed from the Excel file.</summary>
    protected const string EmployeeEmail = "andres.mejia@correo.com";
    protected const string EmployeePassword = "Instructor2026*";
    protected const string ParticipantPassword = "Testing2026*";

    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    protected ApiTestBase(ApiFactory factory)
    {
        Factory = factory;
    }

    protected ApiFactory Factory { get; }

    protected HttpClient Anonymous() => Factory.CreateClient();

    protected async Task<HttpClient> EmployeeAsync() => await SignInAsync(EmployeeEmail, EmployeePassword);

    /// <summary>Registers a brand new participant (unique email) and returns a client signed in as them.</summary>
    protected async Task<(HttpClient Client, AuthResponse Session)> NewParticipantAsync()
    {
        var email = $"participant.{Guid.NewGuid():N}@correo.com";
        var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/register", new
        {
            fullName = "Integration Participant",
            email,
            password = ParticipantPassword
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var session = (await response.Content.ReadFromJsonAsync<AuthResponse>(Json))!;
        return (Authorized(session.AccessToken), session);
    }

    protected async Task<HttpClient> SignInAsync(string email, string password)
    {
        var response = await Anonymous().PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var session = (await response.Content.ReadFromJsonAsync<AuthResponse>(Json))!;
        return Authorized(session.AccessToken);
    }

    /// <summary>Creates a workshop as an employee, starting at the given offset from now (Bogotá time).</summary>
    protected async Task<WorkshopResponse> CreateWorkshopAsync(int capacity, TimeSpan startsIn, TimeSpan? duration = null)
    {
        var start = DateTimeOffset.UtcNow.Add(startsIn).ToOffset(TimeSpan.FromHours(-5));
        var response = await (await EmployeeAsync()).PostAsJsonAsync("/api/v1/workshops", new
        {
            code = UniqueCode(),
            title = "Integration workshop",
            description = "Created by the integration tests",
            instructorEmail = EmployeeEmail,
            startsAt = start,
            endsAt = start.Add(duration ?? TimeSpan.FromHours(3)),
            capacity
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<WorkshopResponse>(Json))!;
    }

    protected static Task<HttpResponseMessage> EnrollAsync(HttpClient participant, Guid workshopId)
        => participant.PostAsJsonAsync("/api/v1/enrollments", new { workshopId });

    protected static async Task<EnrollmentResponse> ReadEnrollmentAsync(HttpResponseMessage response)
        => (await response.Content.ReadFromJsonAsync<EnrollmentResponse>(Json))!;

    /// <summary>Runs a query straight against the test database, to check what the API really stored.</summary>
    protected async Task<T> QueryAsync<T>(Func<ApplicationDbContext, Task<T>> query)
    {
        using var scope = Factory.Services.CreateScope();
        return await query(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
    }

    protected static string UniqueCode() => $"IT-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    private HttpClient Authorized(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
