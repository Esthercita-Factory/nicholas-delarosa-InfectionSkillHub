using InfectionSkillHub.Api.Common;
using InfectionSkillHub.Api.Requests;
using InfectionSkillHub.Application.Common;
using InfectionSkillHub.Application.Dtos.Certificates;
using InfectionSkillHub.Application.Dtos.Enrollments;
using InfectionSkillHub.Application.Services.Certificates;
using InfectionSkillHub.Application.Services.Enrollments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfectionSkillHub.Api.Controllers;

/// <summary>
/// Enrollments. The participant is always taken from the token, never from the request,
/// so nobody can see or act on behalf of another participant.
/// </summary>
[ApiController]
[Route("api/v1/enrollments")]
[Produces("application/json")]
[Tags("Enrollments")]
public sealed class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollments;
    private readonly IEnrollmentImportService _import;
    private readonly ICertificateService _certificates;

    public EnrollmentsController(
        IEnrollmentService enrollments,
        IEnrollmentImportService import,
        ICertificateService certificates)
    {
        _enrollments = enrollments;
        _import = import;
        _certificates = certificates;
    }

    /// <summary>Enrolls the authenticated participant in a workshop.</summary>
    /// <remarks>
    /// Rules: the workshop has not started, there are seats left (only active enrollments count), the
    /// participant has no active enrollment in it and it does not overlap another workshop where they are enrolled.
    /// </remarks>
    /// <response code="201">Enrollment created (Active).</response>
    /// <response code="404">The workshop does not exist.</response>
    /// <response code="409">Already enrolled in this workshop, or it overlaps another active enrollment.</response>
    /// <response code="422">The workshop is full or has already started.</response>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.ParticipantOnly)]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EnrollmentResponse>> Enroll(EnrollRequest request, CancellationToken cancellationToken)
    {
        var result = await _enrollments.EnrollAsync(User.UserId(), request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : this.ProblemFor(result.Error!);
    }

    /// <summary>Enrollments of the authenticated participant, active and cancelled.</summary>
    [HttpGet("mine")]
    [Authorize(Policy = AuthorizationPolicies.ParticipantOnly)]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<EnrollmentResponse>>> Mine(CancellationToken cancellationToken)
        => this.OkOrProblem(await _enrollments.ListMineAsync(User.UserId(), cancellationToken));

    /// <summary>Enrollment detail (its owner or an employee).</summary>
    /// <response code="403">The enrollment belongs to another participant.</response>
    /// <response code="404">The enrollment does not exist.</response>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentResponse>> GetById(Guid id, CancellationToken cancellationToken)
        => this.OkOrProblem(await _enrollments.GetByIdAsync(id, User.ToRequestingUser(), cancellationToken));

    /// <summary>Cancels an enrollment (its owner or an employee).</summary>
    /// <remarks>
    /// A participant can cancel up to 24 hours before the workshop starts. An employee can cancel any
    /// enrollment while the workshop has not started. The cancellation records who did it and when.
    /// </remarks>
    /// <response code="200">Enrollment cancelled; the seat is free again.</response>
    /// <response code="403">The enrollment belongs to another participant.</response>
    /// <response code="404">The enrollment does not exist.</response>
    /// <response code="422">Already cancelled, inside the last 24 hours, or the workshop has started.</response>
    [HttpPatch("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(EnrollmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<EnrollmentResponse>> Cancel(Guid id, CancellationToken cancellationToken)
        => this.OkOrProblem(await _enrollments.CancelAsync(id, User.ToRequestingUser(), cancellationToken));

    /// <summary>Enrolls participants in workshops from an Excel file (employees only).</summary>
    /// <remarks>
    /// Each row (ParticipantEmail, WorkshopCode) is processed as a regular enrollment and must meet every rule.
    /// Valid rows are saved even if others fail. The response tells how many rows were enrolled and, for each
    /// rejected row, its number in the sheet and the reason.
    /// </remarks>
    /// <response code="200">File processed (it may contain rejected rows).</response>
    /// <response code="400">Missing file, not an .xlsx, or without the required columns.</response>
    [HttpPost("import")]
    [Authorize(Policy = AuthorizationPolicies.EmployeeOnly)]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(EnrollmentImportForm.MaxFileBytes + 64 * 1024)]
    [ProducesResponseType(typeof(EnrollmentImportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EnrollmentImportResponse>> Import([FromForm] EnrollmentImportForm form, CancellationToken cancellationToken)
    {
        var file = form.File!;

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            return this.ProblemFor(Error.Validation("Only .xlsx files are accepted.", nameof(EnrollmentImportForm.File)));
        }

        if (file.Length == 0 || file.Length > EnrollmentImportForm.MaxFileBytes)
        {
            return this.ProblemFor(Error.Validation("The file is empty or larger than 5 MB.", nameof(EnrollmentImportForm.File)));
        }

        await using var content = new MemoryStream();
        await file.CopyToAsync(content, cancellationToken);
        content.Position = 0;

        return this.OkOrProblem(await _import.ImportAsync(content, cancellationToken));
    }

    /// <summary>Downloads the enrollment certificate as PDF (its owner or an employee).</summary>
    /// <response code="200">PDF document.</response>
    /// <response code="403">The enrollment belongs to another participant.</response>
    /// <response code="404">The enrollment does not exist.</response>
    /// <response code="422">The enrollment is cancelled; only active enrollments have a certificate.</response>
    [HttpGet("{id:guid}/certificate")]
    [Authorize]
    [Produces(CertificateFile.ContentType, "application/problem+json")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, CertificateFile.ContentType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Certificate(Guid id, CancellationToken cancellationToken)
    {
        var certificate = await _certificates.GetAsync(id, User.ToRequestingUser(), cancellationToken);

        return certificate.IsSuccess
            ? File(certificate.Value!.Content, CertificateFile.ContentType, certificate.Value.FileName)
            : this.ProblemFor(certificate.Error!);
    }
}
