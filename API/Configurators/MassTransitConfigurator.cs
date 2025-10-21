using System.Diagnostics.CodeAnalysis;
using Infra.Utils.Configuration;
using MassTransit;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class MassTransitConfigurator
{
    public static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(Builders.BuildRabbitMQConnectionString(configuration)));
                cfg.UseInstrumentation();
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}