using InfectionSkillHub.Api.Common;
using InfectionSkillHub.Application.Dtos.Reports;
using InfectionSkillHub.Application.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfectionSkillHub.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize(Policy = AuthorizationPolicies.EmployeeOnly)]
[Produces("application/json")]
[Tags("Reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reports;

    public ReportsController(IReportService reports)
    {
        _reports = reports;
    }

    /// <summary>Workshops with the most enrollments, ordered from most to fewest active enrollments (employees only).</summary>
    /// <param name="limit">How many workshops to return (1 to 100, default 10).</param>
    [HttpGet("top-workshops")]
    [ProducesResponseType(typeof(IReadOnlyList<TopWorkshopResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<TopWorkshopResponse>>> TopWorkshops(
        [FromQuery] int limit = IReportService.DefaultLimit,
        CancellationToken cancellationToken = default)
        => Ok(await _reports.TopWorkshopsAsync(limit, cancellationToken));
}
