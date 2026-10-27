using InfectionSkillHub.Api.Common;
using InfectionSkillHub.Application.Dtos.Workshops;
using InfectionSkillHub.Application.Services.Workshops;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfectionSkillHub.Api.Controllers;

/// <summary>Workshop catalog (public) and workshop management (employees).</summary>
[ApiController]
[Route("api/v1/workshops")]
[Produces("application/json")]
[Tags("Workshops")]
public sealed class WorkshopsController : ControllerBase
{
    private readonly IWorkshopService _workshops;

    public WorkshopsController(IWorkshopService workshops)
    {
        _workshops = workshops;
    }

    /// <summary>Lists the workshops that have not started yet, with their available seats.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<WorkshopResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkshopResponse>>> List(CancellationToken cancellationToken)
        => Ok(await _workshops.ListUpcomingAsync(cancellationToken));

    /// <summary>Workshop detail with its available seats.</summary>
    /// <response code="404">The workshop does not exist.</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(WorkshopResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkshopResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => this.OkOrProblem(await _workshops.GetByIdAsync(id, cancellationToken));

    /// <summary>Creates a workshop (employees only).</summary>
    /// <remarks>
    /// The instructor is given by email and must be registered. The start must be in the future and
    /// the end later than the start. Send the dates with their offset, for example 2027-03-06T09:00:00-05:00.
    /// </remarks>
    /// <response code="201">Workshop created.</response>
    /// <response code="400">Invalid data or unknown instructor.</response>
    /// <response code="409">The code is already in use.</response>
    /// <response code="422">The start is not in the future or the end is not after the start.</response>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.EmployeeOnly)]
    [ProducesResponseType(typeof(WorkshopResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<WorkshopResponse>> Create(CreateWorkshopRequest request, CancellationToken cancellationToken)
    {
        var result = await _workshops.CreateAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : this.ProblemFor(result.Error!);
    }

    /// <summary>Lists the participants enrolled in a workshop (employees only).</summary>
    /// <param name="id">Workshop id.</param>
    /// <param name="includeCancelled">Also return cancelled enrollments, with who cancelled them and when.</param>
    /// <response code="404">The workshop does not exist.</response>
    [HttpGet("{id:guid}/enrollments")]
    [Authorize(Policy = AuthorizationPolicies.EmployeeOnly)]
    [ProducesResponseType(typeof(IReadOnlyList<WorkshopEnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<WorkshopEnrollmentResponse>>> Enrollments(
        Guid id,
        [FromQuery] bool includeCancelled,
        CancellationToken cancellationToken)
        => this.OkOrProblem(await _workshops.ListEnrollmentsAsync(id, includeCancelled, cancellationToken));
}
