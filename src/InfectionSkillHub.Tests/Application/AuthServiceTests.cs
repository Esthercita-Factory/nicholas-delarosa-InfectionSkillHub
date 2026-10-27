using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Auth;
using InfectionSkillHub.Application.Services.Auth;
using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Identity;
using InfectionSkillHub.Tests.Common;
using NSubstitute;

namespace InfectionSkillHub.Tests.Application;

public class AuthServiceTests
{
    private readonly IUserService _users = Substitute.For<IUserService>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly IParticipantRepository _participants = Substitute.For<IParticipantRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ITransactionScope _transaction = Substitute.For<ITransactionScope>();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(_transaction);
        _tokens.CreateToken(Arg.Any<UserAccount>(), Arg.Any<IReadOnlyCollection<string>>())
            .Returns(new AccessToken("token", TestData.Now.AddHours(2)));
        _service = new AuthService(_users, _tokens, _participants, _unitOfWork, new FakeClock());
    }

    [Fact]
    public async Task RegisterParticipantAsync_AlwaysCreatesAParticipant()
    {
        var account = new UserAccount("user-id", "ana.gomez@correo.com", "Ana Gómez");
        _users.CreateAsync(Arg.Any<UserRegistration>(), ApplicationRoles.Participant, Arg.Any<CancellationToken>())
            .Returns(UserCreation.Success(account));

        var result = await _service.RegisterParticipantAsync(new RegisterRequest
        {
            FullName = " Ana Gómez ",
            Email = "Ana.Gomez@Correo.com",
            Password = "Skillhub2026*"
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(ApplicationRoles.Participant, result.Value!.User.Role);
        await _users.Received(1).CreateAsync(
            Arg.Is<UserRegistration>(r => r.Email == "ana.gomez@correo.com" && r.FullName == "Ana Gómez"),
            ApplicationRoles.Participant,
            Arg.Any<CancellationToken>());
        await _participants.Received(1).AddAsync(Arg.Is<Participant>(p => p.UserId == "user-id"), Arg.Any<CancellationToken>());
        await _transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterParticipantAsync_RejectsAnEmailInUse()
    {
        _users.EmailExistsAsync("jorge.perez@correo.com", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _service.RegisterParticipantAsync(new RegisterRequest
        {
            FullName = "Jorge",
            Email = "jorge.perez@correo.com",
            Password = "Skillhub2026*"
        });

        Assert.Equal(ErrorCode.Conflict, result.Error!.Code);
        await _users.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!);
    }

    [Fact]
    public async Task RegisterParticipantAsync_RollsBackWhenThePasswordIsRejected()
    {
        _users.CreateAsync(Arg.Any<UserRegistration>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(UserCreation.Failure(["Passwords must have at least one digit."]));

        var result = await _service.RegisterParticipantAsync(new RegisterRequest
        {
            FullName = "Ana",
            Email = "ana@correo.com",
            Password = "Password*"
        });

        Assert.Equal(ErrorCode.Validation, result.Error!.Code);
        await _transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _participants.DidNotReceiveWithAnyArgs().AddAsync(default!);
    }

    [Theory]
    [InlineData(SignInOutcome.InvalidCredentials)]
    [InlineData(SignInOutcome.LockedOut)]
    public async Task LoginAsync_ReturnsUnauthorizedOnFailure(SignInOutcome outcome)
    {
        _users.CheckPasswordAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new SignInAttempt(outcome, null));

        var result = await _service.LoginAsync(new LoginRequest { Email = "x@correo.com", Password = "bad" });

        Assert.Equal(ErrorCode.Unauthorized, result.Error!.Code);
    }

    [Fact]
    public async Task LoginAsync_ReturnsTheEmployeeRole()
    {
        var account = new UserAccount("employee-id", "andres.mejia@correo.com", "Andrés");
        _users.CheckPasswordAsync("andres.mejia@correo.com", "Instructor2026*", Arg.Any<CancellationToken>())
            .Returns(new SignInAttempt(SignInOutcome.Success, account));
        _users.GetRolesAsync("employee-id", Arg.Any<CancellationToken>()).Returns([ApplicationRoles.Employee]);

        var result = await _service.LoginAsync(new LoginRequest { Email = "Andres.Mejia@correo.com", Password = "Instructor2026*" });

        Assert.Equal("token", result.Value!.AccessToken);
        Assert.Equal(ApplicationRoles.Employee, result.Value.User.Role);
    }
}
