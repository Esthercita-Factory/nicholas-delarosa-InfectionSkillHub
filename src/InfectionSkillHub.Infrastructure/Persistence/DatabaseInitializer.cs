using InfectionSkillHub.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InfectionSkillHub.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    /// <summary>
    /// Applies the pending migrations (creating the database if needed) and loads the initial
    /// InfectionSkillHub data. The schema only comes from the EF Core migrations, never from SQL scripts.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        await provider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync(cancellationToken);

        if (provider.GetRequiredService<IConfiguration>().GetValue("Seed:Enabled", true))
        {
            await provider.GetRequiredService<InfectionSkillHubDataSeeder>().SeedAsync(cancellationToken);
        }
    }
}
