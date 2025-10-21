using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Infra.Utils.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class KeycloakConfigurator
{
    public static IServiceCollection AddKeycloakConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloak = configuration.GetSection("Keycloak").Get<Keycloak>();
        if (keycloak == null) throw new ArgumentException("Authorization not provided");
        services.Configure<Keycloak>(configuration.GetSection("Keycloak"));
        
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = keycloak.Issuer;
                options.Audience = keycloak.Audience;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = keycloak.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = keycloak.Issuer,
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };
            });
        services.AddAuthorization();
        return services;
    }
}