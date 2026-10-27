using System.Net;
using System.Net.Http.Json;
using System.Text;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Application.Dtos.Workshops;
using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace InfectionSkillHub.IntegrationTests.Api;

public class EnrollmentLifecycleTests : ApiTestBase
{
    public EnrollmentLifecycleTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Cancel_ReleasesTheSeatAndAllowsReEnrollment()
    {
        var workshop = await CreateWorkshopAsync(capacity: 1, startsIn: TimeSpan.FromDays(60));
        var (owner, _) = await NewParticipantAsync();
        var (other, _) = await NewParticipantAsync();

        var first = await ReadEnrollmentAsync(await EnrollAsync(owner, workshop.Id));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await EnrollAsync(other, workshop.Id)).StatusCode);

        var cancelled = await ReadEnrollmentAsync(await owner.PatchAsync($"/api/v1/enrollments/{first.Id}/cancel", null));
        Assert.Equal(EnrollmentStatus.Cancelled, cancelled.Status);
        Assert.NotNull(cancelled.CancelledAt);

        Assert.Equal(HttpStatusCode.Created, (await EnrollAsync(other, workshop.Id)).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await EnrollAsync(owner, workshop.Id)).StatusCode);
    }

    [Fact]
    public async Task ReEnrollment_KeepsTheCancelledEnrollmentAsHistory()
    {
        var workshop = await CreateWorkshopAsync(capacity: 5, startsIn: TimeSpan.FromDays(61));
        var (participant, session) = await NewParticipantAsync();

        var first = await ReadEnrollmentAsync(await EnrollAsync(participant, workshop.Id));
        await participant.PatchAsync($"/api/v1/enrollments/{first.Id}/cancel", null);
        Assert.Equal(HttpStatusCode.Created, (await EnrollAsync(participant, workshop.Id)).StatusCode);

        var statuses = await QueryAsync(db => db.Enrollments
            .Where(e => e.WorkshopId == workshop.Id && e.Participant.Email == session.User.Email)
            .Select(e => e.Status)
            .ToListAsync());
        Assert.Equal([EnrollmentStatus.Active, EnrollmentStatus.Cancelled], statuses.Order());
    }

    [Fact]
    public async Task InsideTheLast24Hours_OnlyAnEmployeeCanCancel_AndTheCancellationIsAudited()
    {
        var workshop = await CreateWorkshopAsync(capacity: 5, startsIn: TimeSpan.FromHours(5));
        var (participant, _) = await NewParticipantAsync();
        var enrollment = await ReadEnrollmentAsync(await EnrollAsync(participant, workshop.Id));
        Assert.False(enrollment.CanCancel);

        var byParticipant = await participant.PatchAsync($"/api/v1/enrollments/{enrollment.Id}/cancel", null);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, byParticipant.StatusCode);

        var byEmployee = await (await EmployeeAsync()).PatchAsync($"/api/v1/enrollments/{enrollment.Id}/cancel", null);
        Assert.Equal(HttpStatusCode.OK, byEmployee.StatusCode);

        var stored = await QueryAsync(db => db.Enrollments.AsNoTracking().SingleAsync(e => e.Id == enrollment.Id));
        Assert.Equal(EnrollmentStatus.Cancelled, stored.Status);
        Assert.Equal(EmployeeEmail, stored.CancelledByEmail);
        Assert.NotNull(stored.CancelledByUserId);
        Assert.NotNull(stored.CancelledAt);
    }

    [Fact]
    public async Task AnotherParticipant_CannotReadCancelOrDownloadTheEnrollment()
    {
        var workshop = await CreateWorkshopAsync(capacity: 5, startsIn: TimeSpan.FromDays(62));
        var (owner, _) = await NewParticipantAsync();
        var (intruder, _) = await NewParticipantAsync();
        var enrollment = await ReadEnrollmentAsync(await EnrollAsync(owner, workshop.Id));

        Assert.Equal(HttpStatusCode.Forbidden, (await intruder.GetAsync($"/api/v1/enrollments/{enrollment.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await intruder.PatchAsync($"/api/v1/enrollments/{enrollment.Id}/cancel", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await intruder.GetAsync($"/api/v1/enrollments/{enrollment.Id}/certificate")).StatusCode);

        var mine = await intruder.GetFromJsonAsync<List<EnrollmentResponse>>("/api/v1/enrollments/mine", Json);
        Assert.DoesNotContain(mine!, e => e.Id == enrollment.Id);
    }

    [Fact]
    public async Task Certificate_IsAPdfForActiveEnrollmentsOnly()
    {
        var workshop = await CreateWorkshopAsync(capacity: 5, startsIn: TimeSpan.FromDays(63));
        var (participant, _) = await NewParticipantAsync();
        var enrollment = await ReadEnrollmentAsync(await EnrollAsync(participant, workshop.Id));

        var pdf = await participant.GetAsync($"/api/v1/enrollments/{enrollment.Id}/certificate");
        Assert.Equal(HttpStatusCode.OK, pdf.StatusCode);
        Assert.Equal("application/pdf", pdf.Content.Headers.ContentType?.MediaType);
        Assert.Equal("%PDF", Encoding.ASCII.GetString((await pdf.Content.ReadAsByteArrayAsync())[..4]));

        var byEmployee = await (await EmployeeAsync()).GetAsync($"/api/v1/enrollments/{enrollment.Id}/certificate");
        Assert.Equal(HttpStatusCode.OK, byEmployee.StatusCode);

        await participant.PatchAsync($"/api/v1/enrollments/{enrollment.Id}/cancel", null);
        var afterCancel = await participant.GetAsync($"/api/v1/enrollments/{enrollment.Id}/certificate");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, afterCancel.StatusCode);
    }

    [Fact]
    public async Task Employee_SeesTheEnrollmentsOfAWorkshop()
    {
        var workshop = await CreateWorkshopAsync(capacity: 5, startsIn: TimeSpan.FromDays(64));
        var (participant, session) = await NewParticipantAsync();
        var enrollment = await ReadEnrollmentAsync(await EnrollAsync(participant, workshop.Id));
        await participant.PatchAsync($"/api/v1/enrollments/{enrollment.Id}/cancel", null);
        await EnrollAsync(participant, workshop.Id);

        var employee = await EmployeeAsync();
        var active = await employee.GetFromJsonAsync<List<WorkshopEnrollmentResponse>>($"/api/v1/workshops/{workshop.Id}/enrollments", Json);
        var all = await employee.GetFromJsonAsync<List<WorkshopEnrollmentResponse>>($"/api/v1/workshops/{workshop.Id}/enrollments?includeCancelled=true", Json);

        Assert.Equal(session.User.Email, Assert.Single(active!).ParticipantEmail);
        Assert.Equal(2, all!.Count);
        Assert.Contains(all, e => e.Status == EnrollmentStatus.Cancelled && e.CancelledBy == session.User.Email);
    }
}
