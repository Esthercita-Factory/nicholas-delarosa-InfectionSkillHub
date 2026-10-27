using InfectionSkillHub.Application.Dtos.Reports;

namespace InfectionSkillHub.Application.Services.Reports;

public interface IReportService
{
    public const int DefaultLimit = 10;
    public const int MaxLimit = 100;

    /// <summary>Workshops with the most active enrollments, from most to fewest.</summary>
    Task<IReadOnlyList<TopWorkshopResponse>> TopWorkshopsAsync(int limit, CancellationToken cancellationToken = default);
}
