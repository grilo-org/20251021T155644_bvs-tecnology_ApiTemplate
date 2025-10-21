using System.Diagnostics.CodeAnalysis;
using Infra.Utils.Configuration;
using Microsoft.OpenApi.Models;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class OpenApiConfigurator
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloak = configuration.GetSection("Keycloak").Get<Keycloak>();
        if (keycloak == null) throw new ArgumentException("Authorization not provided");
        
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes["oauth2"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{keycloak.Issuer}/protocol/openid-connect/auth"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "openid" },
                                { "profile", "profile" },
                            }
                        }
                    }
                };

                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    [document.Components.SecuritySchemes["oauth2"]] = new List<string> { "openid" }
                });

                return Task.CompletedTask;
            });
        });
        return services;
    }
}