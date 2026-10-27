using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace InfectionSkillHub.IntegrationTests.Common;

/// <summary>
/// The real API (migrations, seed, JWT, controllers) running against a throwaway PostgreSQL container.
/// One instance is shared by every test class of the "Api" collection.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string SigningKey = "integration-tests-signing-key-0123456789abcdef";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("infection_skillhub_tests")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        // Forces the host to start now: migrations and the Excel seed run before the first test.
        _ = Server;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", _postgres.GetConnectionString());
        builder.UseSetting("Jwt:SigningKey", SigningKey);
        builder.UseSetting("Database:MigrateOnStartup", "true");
        builder.UseSetting("Seed:Enabled", "true");
        builder.UseSetting("OpenApi:Enabled", "false");
    }
}

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "Api";
}
