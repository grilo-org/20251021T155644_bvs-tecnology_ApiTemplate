using System.Diagnostics.CodeAnalysis;
using Application.Services;
using Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;
[ExcludeFromCodeCoverage]
public static class ApplicationInjector
{
    public static IServiceCollection InjectApplication(this IServiceCollection services)
    {
        services.InjectServices();
        
        return services;
    }
    
    private static IServiceCollection InjectServices(this IServiceCollection services)
    {
        services.AddScoped<ITestService, TestService>();
        
        return services;
    }
}