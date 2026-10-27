using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Errors;
using InfectionSkillHub.Tests.Common;

namespace InfectionSkillHub.Tests.Domain;

public class WorkshopTests
{
    [Fact]
    public void Schedule_NormalizesTheCodeAndStoresDatesInUtc()
    {
        var localStart = new DateTimeOffset(2026, 12, 5, 8, 0, 0, TimeSpan.FromHours(-5));

        var workshop = Workshop.Schedule(" fot-010 ", "Title", "Description", TestData.Instructor(), localStart, localStart.AddHours(4), 15, TestData.Now);

        Assert.Equal("FOT-010", workshop.Code);
        Assert.Equal(TimeSpan.Zero, workshop.StartsAt.Offset);
        Assert.Equal(new DateTimeOffset(2026, 12, 5, 13, 0, 0, TimeSpan.Zero), workshop.StartsAt);
    }

    [Fact]
    public void Schedule_RejectsAStartInThePast()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => TestData.Workshop(startsIn: TimeSpan.FromMinutes(-1)));

        Assert.Contains("future", exception.Message);
    }

    [Fact]
    public void Schedule_RejectsAStartEqualToNow()
        => Assert.Throws<BusinessRuleViolationException>(() => TestData.Workshop(startsIn: TimeSpan.Zero));

    [Theory]
    [InlineData(0)]
    [InlineData(-60)]
    public void Schedule_RequiresTheEndAfterTheStart(int durationMinutes)
        => Assert.Throws<BusinessRuleViolationException>(() => TestData.Workshop(duration: TimeSpan.FromMinutes(durationMinutes)));

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(Workshop.MaxCapacity + 1)]
    public void Schedule_RejectsAnInvalidCapacity(int capacity)
        => Assert.Throws<DomainException>(() => TestData.Workshop(capacity: capacity));

    [Theory]
    [InlineData("FOT 001")]
    [InlineData("FOT_001")]
    [InlineData("-FOT")]
    public void Schedule_RejectsAnInvalidCode(string code)
        => Assert.Throws<DomainException>(() => TestData.Workshop(code: code));

    [Fact]
    public void Restore_AcceptsWorkshopsThatAlreadyStarted()
    {
        var start = TestData.Now.AddDays(-2);

        var workshop = Workshop.Restore("OLD-001", "Title", "Description", TestData.Instructor(), start, start.AddHours(2), 10, TestData.Now);

        Assert.True(workshop.HasStarted(TestData.Now));
    }

    [Fact]
    public void OverlapsWith_DetectsPartialOverlap()
    {
        var morning = TestData.Workshop("A-1", TimeSpan.FromHours(24), TimeSpan.FromHours(4));
        var midday = TestData.Workshop("B-1", TimeSpan.FromHours(27), TimeSpan.FromHours(2));

        Assert.True(morning.OverlapsWith(midday));
        Assert.True(midday.OverlapsWith(morning));
    }

    [Fact]
    public void OverlapsWith_DetectsAWorkshopContainedInAnother()
    {
        var day = TestData.Workshop("A-1", TimeSpan.FromHours(24), TimeSpan.FromHours(8));
        var inside = TestData.Workshop("B-1", TimeSpan.FromHours(26), TimeSpan.FromHours(1));

        Assert.True(day.OverlapsWith(inside));
    }

    [Fact]
    public void OverlapsWith_IgnoresBackToBackWorkshops()
    {
        var first = TestData.Workshop("A-1", TimeSpan.FromHours(24), TimeSpan.FromHours(2));
        var second = TestData.Workshop("B-1", TimeSpan.FromHours(26), TimeSpan.FromHours(2));

        Assert.False(first.OverlapsWith(second));
        Assert.False(second.OverlapsWith(first));
    }

    [Fact]
    public void CancellationDeadline_Is24HoursBeforeTheStart()
    {
        var workshop = TestData.Workshop(startsIn: TimeSpan.FromDays(3));

        Assert.Equal(workshop.StartsAt.AddHours(-24), workshop.CancellationDeadline);
    }
}
