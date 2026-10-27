using InfectionSkillHub.Application.Abstractions;
using InfectionSkillHub.Application.Dtos.Reports;
using InfectionSkillHub.Application.Mappings;

namespace InfectionSkillHub.Application.Services.Reports;

public sealed class ReportService : IReportService
{
    private readonly IWorkshopRepository _workshops;

    public ReportService(IWorkshopRepository workshops)
    {
        _workshops = workshops;
    }

    public async Task<IReadOnlyList<TopWorkshopResponse>> TopWorkshopsAsync(int limit, CancellationToken cancellationToken = default)
    {
        var top = await _workshops.ListTopByActiveEnrollmentsAsync(Math.Clamp(limit, 1, IReportService.MaxLimit), cancellationToken);
        return top.Select((seats, index) => seats.ToTopWorkshopResponse(index + 1)).ToList();
    }
}
