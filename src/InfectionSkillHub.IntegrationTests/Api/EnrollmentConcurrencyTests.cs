using System.Net;
using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace InfectionSkillHub.IntegrationTests.Api;

/// <summary>
/// Simultaneous requests against real PostgreSQL. These are the cases the row locks and the partial
/// unique index exist for; unit tests with substitutes cannot reach them.
/// </summary>
public class EnrollmentConcurrencyTests : ApiTestBase
{
    public EnrollmentConcurrencyTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ManyParticipants_RacingForTheLastSeats_NeverOverbookTheWorkshop()
    {
        const int capacity = 2;
        const int contenders = 10;
        var workshop = await CreateWorkshopAsync(capacity, TimeSpan.FromDays(50));
        var participants = await Task.WhenAll(Enumerable.Range(0, contenders).Select(_ => NewParticipantAsync()));

        var responses = await Task.WhenAll(participants.Select(p => EnrollAsync(p.Client, workshop.Id)));

        Assert.Equal(capacity, responses.Count(r => r.StatusCode == HttpStatusCode.Created));
        Assert.All(responses.Where(r => r.StatusCode != HttpStatusCode.Created),
            r => Assert.Equal(HttpStatusCode.UnprocessableEntity, r.StatusCode));
        Assert.Equal(capacity, await ActiveEnrollmentsAsync(workshop.Id));
    }

    [Fact]
    public async Task SameParticipant_DoubleSubmitting_GetsOneEnrollment()
    {
        var workshop = await CreateWorkshopAsync(capacity: 10, startsIn: TimeSpan.FromDays(51));
        var (participant, _) = await NewParticipantAsync();

        var responses = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => EnrollAsync(participant, workshop.Id)));

        Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Created);
        Assert.All(responses.Where(r => r.StatusCode != HttpStatusCode.Created),
            r => Assert.Equal(HttpStatusCode.Conflict, r.StatusCode));
        Assert.Equal(1, await ActiveEnrollmentsAsync(workshop.Id));
    }

    [Fact]
    public async Task SameParticipant_EnrollingInTwoOverlappingWorkshopsAtOnce_GetsOnlyOne()
    {
        var first = await CreateWorkshopAsync(capacity: 10, startsIn: TimeSpan.FromDays(52), duration: TimeSpan.FromHours(3));
        var overlapping = await CreateWorkshopAsync(capacity: 10, startsIn: TimeSpan.FromDays(52) + TimeSpan.FromHours(1), duration: TimeSpan.FromHours(3));
        var (participant, session) = await NewParticipantAsync();

        var responses = await Task.WhenAll(EnrollAsync(participant, first.Id), EnrollAsync(participant, overlapping.Id));

        Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Created);
        Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Conflict);
        var active = await QueryAsync(db => db.Enrollments.CountAsync(e =>
            e.Participant.Email == session.User.Email && e.Status == EnrollmentStatus.Active));
        Assert.Equal(1, active);
    }

    private Task<int> ActiveEnrollmentsAsync(Guid workshopId)
        => QueryAsync(db => db.Enrollments.CountAsync(e => e.WorkshopId == workshopId && e.Status == EnrollmentStatus.Active));
}
