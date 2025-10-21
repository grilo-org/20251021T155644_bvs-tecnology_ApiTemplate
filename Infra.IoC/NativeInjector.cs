using System.Diagnostics.CodeAnalysis;
using Application;
using Domain.SeedWork.Notification;
using Infra.Data;
using Infra.Http;
using Infra.Utils.Transformers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.IoC;
[ExcludeFromCodeCoverage]
public static class NativeInjector
{
    public static IServiceCollection InjectDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .InjectData(configuration)
            .InjectHttp()
            .InjectApplication();
        
        services.AddScoped<INotification, Notification>();
        services.AddScoped<IClaimsTransformation, KeycloakClaimsTransformer>();
        
        return services;
    }
}
