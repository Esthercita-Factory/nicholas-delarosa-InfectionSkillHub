using System.Text.Json.Serialization;
using InfectionSkillHub.Api.Common;
using InfectionSkillHub.Api.OpenApi;
using InfectionSkillHub.Application;
using InfectionSkillHub.Domain.Identity;
using InfectionSkillHub.Infrastructure;
using InfectionSkillHub.Infrastructure.Persistence;
using InfectionSkillHub.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

const string CorsPolicy = "Clients";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((options, jwt) =>
    {
        // Short claim names ("sub", "role") without the WS-Federation URI mapping.
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Value.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JwtTokenService.SigningKey(jwt.Value),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = JwtTokenService.NameClaim,
            RoleClaimType = JwtTokenService.RoleClaim
        };
    });

// Everything requires a token except what is marked with [AllowAnonymous].
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthorizationPolicies.EmployeeOnly, policy => policy.RequireRole(ApplicationRoles.Employee))
    .AddPolicy(AuthorizationPolicies.ParticipantOnly, policy => policy.RequireRole(ApplicationRoles.Participant))
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services
    .AddControllers(options =>
    {
        // Validation errors use the JSON name of the field ("email", not "Email").
        options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid data",
            Detail = "One or more fields are not valid.",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problem) { ContentTypes = { "application/problem+json" } };
    };
});

// 401 and 403 without a body are also returned as ProblemDetails with a useful message.
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
{
    switch (context.ProblemDetails.Status)
    {
        case StatusCodes.Status401Unauthorized:
            context.ProblemDetails.Title = "Not authenticated";
            context.ProblemDetails.Detail ??= "The token is missing or has expired. Sign in again.";
            break;
        case StatusCodes.Status403Forbidden:
            context.ProblemDetails.Title = "Access denied";
            context.ProblemDetails.Detail ??= "Your role is not allowed to perform this operation.";
            break;
    }
});
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy(CorsPolicy, policy => policy
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()
    // Without it the browser cannot read the file name of the downloaded PDF.
    .WithExposedHeaders("Content-Disposition")));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<BearerSecurityRequirementTransformer>();
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Configuration.GetValue("OpenApi:Enabled", app.Environment.IsDevelopment()))
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference(options => options
            .WithTitle("InfectionSkillHub API")
            .WithTheme(ScalarTheme.DeepSpace)
            .SortTagsAlphabetically()
            .AddPreferredSecuritySchemes(BearerSecuritySchemeTransformer.SchemeName)
            .EnablePersistentAuthentication())
        .AllowAnonymous();

    app.MapGet("/", () => Results.Redirect("/scalar")).AllowAnonymous().ExcludeFromDescription();
}

if (app.Configuration.GetValue("UseHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", async (ApplicationDbContext db, CancellationToken cancellationToken) =>
        await db.Database.CanConnectAsync(cancellationToken)
            ? Results.Ok(new { status = "ok" })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable))
    .AllowAnonymous()
    .WithTags("Health");

if (app.Configuration.GetValue("Database:MigrateOnStartup", true))
{
    await DatabaseInitializer.InitializeAsync(app.Services);
}

app.Run();
