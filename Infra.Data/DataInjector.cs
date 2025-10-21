using System.Diagnostics.CodeAnalysis;
using Domain.Interfaces.Repositories;
using Infra.Data.Context;
using Infra.Data.Repositories;
using Infra.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Data;
[ExcludeFromCodeCoverage]
public static class DataInjector
{
    public static IServiceCollection InjectData(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .InjectUnitOfWork(configuration)
            .InjectCache(configuration)
            .InjectRepositories();
        return services;
    }
    
    private static IServiceCollection InjectUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context.Context>(options => 
            options.UseLazyLoadingProxies().UseNpgsql(Builders.BuildPostgresConnectionString(configuration)));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
    
    private static IServiceCollection InjectCache(this IServiceCollection services, IConfiguration configuration) {
        services.AddStackExchangeRedisCache(options => options.Configuration = Builders.BuildRedisConnectionString(configuration));
        return services;
    }
    
    private static IServiceCollection InjectRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITestRepository, TestRepository>();
        return services;
    }
}