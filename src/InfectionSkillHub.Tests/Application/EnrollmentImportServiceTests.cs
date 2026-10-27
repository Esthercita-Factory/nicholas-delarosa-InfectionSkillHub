using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Services.Enrollments;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Tests.Common;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace InfectionSkillHub.Tests.Application;

public class EnrollmentImportServiceTests
{
    private readonly IEnrollmentSheetReader _reader = Substitute.For<IEnrollmentSheetReader>();
    private readonly IEnrollmentRegistrar _registrar = Substitute.For<IEnrollmentRegistrar>();
    private readonly IParticipantRepository _participants = Substitute.For<IParticipantRepository>();
    private readonly IWorkshopRepository _workshops = Substitute.For<IWorkshopRepository>();
    private readonly EnrollmentImportService _service;

    public EnrollmentImportServiceTests()
    {
        _service = new EnrollmentImportService(_reader, _registrar, _participants, _workshops, NullLogger<EnrollmentImportService>.Instance);
    }

    [Fact]
    public async Task ImportAsync_SavesValidRowsAndReportsEachRejectedOne()
    {
        var jorge = TestData.Participant("jorge.perez@correo.com");
        var luz = TestData.Participant("luz.ospina@correo.com");
        var workshop = TestData.Workshop("COC-001");
        Rows(
            new EnrollmentSheetRow(2, "jorge.perez@correo.com", "coc-001"),
            new EnrollmentSheetRow(3, "nobody@correo.com", "COC-001"),
            new EnrollmentSheetRow(4, "luz.ospina@correo.com", "XXX-999"),
            new EnrollmentSheetRow(5, "luz.ospina@correo.com", "COC-001"),
            new EnrollmentSheetRow(6, "", "COC-001"));
        _participants.FindByEmailAsync("jorge.perez@correo.com", Arg.Any<CancellationToken>()).Returns(jorge);
        _participants.FindByEmailAsync("luz.ospina@correo.com", Arg.Any<CancellationToken>()).Returns(luz);
        _workshops.FindByCodeAsync("COC-001", Arg.Any<CancellationToken>()).Returns(workshop);
        _registrar.EnrollAsync(jorge, workshop, Arg.Any<CancellationToken>()).Returns(Result.Success(TestData.Enrollment(jorge, workshop)));
        _registrar.EnrollAsync(luz, workshop, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Enrollment>(Error.BusinessRule("Workshop COC-001 is full: all 1 seats are taken.")));

        var result = await _service.ImportAsync(Stream.Null);

        var response = result.Value!;
        Assert.Equal(5, response.TotalRows);
        Assert.Equal(1, response.EnrolledCount);
        Assert.Equal(4, response.RejectedCount);
        Assert.Collection(response.RejectedRows,
            row => Assert.Equal((3, "There is no participant registered with the email nobody@correo.com."), (row.RowNumber, row.Reason)),
            row => Assert.Equal((4, "Workshop XXX-999 does not exist."), (row.RowNumber, row.Reason)),
            row => Assert.Equal((5, "Workshop COC-001 is full: all 1 seats are taken."), (row.RowNumber, row.Reason)),
            row => Assert.Equal((6, "ParticipantEmail is empty."), (row.RowNumber, row.Reason)));
    }

    [Fact]
    public async Task ImportAsync_FailsWhenTheFileCannotBeRead()
    {
        _reader.Read(Arg.Any<Stream>()).Returns(Result.Failure<IReadOnlyList<EnrollmentSheetRow>>(Error.Validation("bad file", "File")));

        var result = await _service.ImportAsync(Stream.Null);

        Assert.Equal(ErrorCode.Validation, result.Error!.Code);
        await _registrar.DidNotReceiveWithAnyArgs().EnrollAsync(default!, default!);
    }

    [Fact]
    public async Task ImportAsync_FailsWhenTheFileHasNoRows()
    {
        Rows();

        var result = await _service.ImportAsync(Stream.Null);

        Assert.Equal(ErrorCode.Validation, result.Error!.Code);
    }

    private void Rows(params EnrollmentSheetRow[] rows)
        => _reader.Read(Arg.Any<Stream>()).Returns(Result.Success<IReadOnlyList<EnrollmentSheetRow>>(rows));
}
