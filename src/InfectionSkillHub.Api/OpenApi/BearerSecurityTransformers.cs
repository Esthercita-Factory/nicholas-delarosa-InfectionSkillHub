using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace InfectionSkillHub.Api.OpenApi;

/// <summary>Declares the JWT bearer scheme so Scalar shows the token field.</summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeName = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "InfectionSkillHub API",
            Version = "v1",
            Description = "InfectionSkillHub workshops and enrollments. Sign in with POST /api/v1/auth/login, copy the accessToken "
                + "and paste it in the Authentication section (Bearer token). Employee credentials are listed in the README."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT returned by /api/v1/auth/login or /api/v1/auth/register."
        };

        return Task.CompletedTask;
    }
}

/// <summary>Marks every endpoint that is not [AllowAnonymous] as requiring the bearer token.</summary>
public sealed class BearerSecurityRequirementTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var isAnonymous = context.Description.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any();
        if (!isAnonymous)
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerSecuritySchemeTransformer.SchemeName, context.Document)] = []
            });
        }

        return Task.CompletedTask;
    }
}
