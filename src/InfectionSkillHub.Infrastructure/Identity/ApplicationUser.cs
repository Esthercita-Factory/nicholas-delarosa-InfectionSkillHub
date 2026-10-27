using Microsoft.AspNetCore.Identity;

namespace InfectionSkillHub.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
