using InfectionSkillHub.Api.Common;
using InfectionSkillHub.Application.Dtos.Auth;
using InfectionSkillHub.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfectionSkillHub.Api.Controllers;

/// <summary>Sign up and sign in with JWT.</summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
[Tags("Auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Registers a new participant.</summary>
    /// <remarks>Whoever signs up is always a participant. The response already carries the token.</remarks>
    /// <response code="201">Account created.</response>
    /// <response code="400">Invalid data or a password that does not meet the policy.</response>
    /// <response code="409">The email is already registered.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _auth.RegisterParticipantAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Me), null, result.Value)
            : this.ProblemFor(result.Error!);
    }

    /// <summary>Signs in and returns the JWT (send it as "Authorization: Bearer {token}").</summary>
    /// <response code="200">Valid credentials.</response>
    /// <response code="401">Wrong email or password, or account locked after repeated failures.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
        => this.OkOrProblem(await _auth.LoginAsync(request, cancellationToken));

    /// <summary>Profile of the authenticated user.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfile>> Me(CancellationToken cancellationToken)
        => this.OkOrProblem(await _auth.GetProfileAsync(User.UserId(), cancellationToken));
}
