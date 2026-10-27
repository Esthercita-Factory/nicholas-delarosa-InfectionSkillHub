using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Workshops;
using InfectionSkillHub.Application.Services.Workshops;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Tests.Common;
using NSubstitute;

namespace InfectionSkillHub.Tests.Application;

public class WorkshopServiceTests
{
    private readonly IWorkshopRepository _workshops = Substitute.For<IWorkshopRepository>();
    private readonly IInstructorRepository _instructors = Substitute.For<IInstructorRepository>();
    private readonly IEnrollmentRepository _enrollments = Substitute.For<IEnrollmentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly WorkshopService _service;

    public WorkshopServiceTests()
    {
        _service = new WorkshopService(_workshops, _instructors, _enrollments, _unitOfWork, new FakeClock());
    }

    [Fact]
    public async Task CreateAsync_RequiresARegisteredInstructor()
    {
        var result = await _service.CreateAsync(Request());

        Assert.Equal(ErrorCode.Validation, result.Error!.Code);
        Assert.Equal(nameof(CreateWorkshopRequest.InstructorEmail), result.Error.Field);
    }

    [Fact]
    public async Task CreateAsync_RejectsADuplicatedCode()
    {
        _instructors.FindByEmailAsync("andres.mejia@correo.com", Arg.Any<CancellationToken>()).Returns(TestData.Instructor());
        _workshops.CodeExistsAsync("FOT-009", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _service.CreateAsync(Request());

        Assert.Equal(ErrorCode.Conflict, result.Error!.Code);
    }

    [Fact]
    public async Task CreateAsync_RejectsAPastStart()
    {
        _instructors.FindByEmailAsync("andres.mejia@correo.com", Arg.Any<CancellationToken>()).Returns(TestData.Instructor());

        var result = await _service.CreateAsync(Request(startsAt: TestData.Now.AddDays(-1)));

        Assert.Equal(ErrorCode.BusinessRule, result.Error!.Code);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_SavesTheWorkshopWithAllSeatsAvailable()
    {
        _instructors.FindByEmailAsync("andres.mejia@correo.com", Arg.Any<CancellationToken>()).Returns(TestData.Instructor());

        var result = await _service.CreateAsync(Request());

        Assert.True(result.IsSuccess);
        Assert.Equal("FOT-009", result.Value!.Code);
        Assert.Equal(12, result.Value.AvailableSeats);
        await _workshops.Received(1).AddAsync(Arg.Is<Workshop>(w => w.Code == "FOT-009"), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListUpcomingAsync_CalculatesAvailableSeats()
    {
        var workshop = TestData.Workshop(capacity: 10);
        _workshops.ListUpcomingAsync(TestData.Now, Arg.Any<CancellationToken>()).Returns([new WorkshopSeats(workshop, 4)]);

        var catalog = await _service.ListUpcomingAsync();

        Assert.Equal(6, Assert.Single(catalog).AvailableSeats);
    }

    private static CreateWorkshopRequest Request(DateTimeOffset? startsAt = null)
    {
        var start = startsAt ?? TestData.Now.AddDays(20);
        return new CreateWorkshopRequest
        {
            Code = "fot-009",
            Title = "Fotografía nocturna",
            Description = "Exposición larga",
            InstructorEmail = "Andres.Mejia@correo.com",
            StartsAt = start,
            EndsAt = start.AddHours(3),
            Capacity = 12
        };
    }
}
