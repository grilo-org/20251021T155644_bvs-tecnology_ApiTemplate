using System.Diagnostics.CodeAnalysis;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class OpenTelemetryConfigurator
{
    public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var apiName = configuration["ApiName"]!;
        var otelUri = configuration["OpenTelemetryUrl"]!;
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(apiName);
        services.AddOpenTelemetry()
            .WithTracing(traceBuilder =>
            {
                traceBuilder
                    .AddSource(apiName)
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMassTransitInstrumentation()
                    .AddSource("MassTransit")
                    .AddNpgsql()
                    .AddOtlpExporter(opt => opt.Endpoint = new Uri(otelUri));
            })
            .WithMetrics(metricsBuilder =>
            {
                metricsBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .SetResourceBuilder(resourceBuilder)
                    .AddOtlpExporter(opt => opt.Endpoint = new Uri(otelUri));
            });
        
        return services;
    }

    public static ILoggingBuilder AddOpenTelemetryConfiguration(this ILoggingBuilder logging, IConfiguration configuration)
    {
        var apiName = configuration["ApiName"]!;
        var otelUri = configuration["OpenTelemetryUrl"]!;
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(apiName);
        logging.AddOpenTelemetry(loggingBuilder =>
        {
            loggingBuilder.IncludeFormattedMessage = true;
            loggingBuilder.SetResourceBuilder(resourceBuilder)
                .AttachLogsToActivityEvent()
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(otelUri));
        });
        return logging;
    }
}