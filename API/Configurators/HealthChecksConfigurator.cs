using System.Diagnostics.CodeAnalysis;
using Infra.Utils.Configuration;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class HealthChecksConfigurator
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(Builders.BuildPostgresConnectionString(configuration))
            .AddRedis(Builders.BuildRedisConnectionString(configuration));
        return services;
    }
}