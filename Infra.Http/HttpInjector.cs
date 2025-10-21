using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Http;
[ExcludeFromCodeCoverage]
public static class HttpInjector
{
    public static IServiceCollection InjectHttp(this IServiceCollection services)
    {
        services.InjectCrossCutting();
        return services;
    }
    
    private static IServiceCollection InjectCrossCutting(this IServiceCollection services)
    {
        return services;
    }
}