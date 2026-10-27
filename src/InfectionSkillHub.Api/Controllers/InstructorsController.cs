using InfectionSkillHub.Api.Common;
using InfectionSkillHub.Application.Dtos.Instructors;
using InfectionSkillHub.Application.Services.Instructors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfectionSkillHub.Api.Controllers;

[ApiController]
[Route("api/v1/instructors")]
[Authorize(Policy = AuthorizationPolicies.EmployeeOnly)]
[Produces("application/json")]
[Tags("Instructors")]
public sealed class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructors;

    public InstructorsController(IInstructorService instructors)
    {
        _instructors = instructors;
    }

    /// <summary>Registered instructors, to know which emails can teach a new workshop (employees only).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<InstructorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<InstructorResponse>>> List(CancellationToken cancellationToken)
        => Ok(await _instructors.ListAsync(cancellationToken));
}
