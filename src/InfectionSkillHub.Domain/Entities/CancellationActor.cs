namespace InfectionSkillHub.Domain.Entities;

/// <summary>Who cancels an enrollment. Employees are not bound by the 24 hour notice.</summary>
public sealed record CancellationActor(string UserId, string Email, bool IsEmployee);
