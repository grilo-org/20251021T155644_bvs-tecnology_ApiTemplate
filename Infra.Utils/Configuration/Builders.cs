using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StackExchange.Redis;

namespace Infra.Utils.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class Builders
    {
        public static string BuildPostgresConnectionString(IConfiguration configuration)
        {
            var environmentConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
            var configurationConnectionString = configuration.GetConnectionString("Postgres");
            
            if (string.IsNullOrWhiteSpace(environmentConnectionString) && string.IsNullOrWhiteSpace(configurationConnectionString))
                throw new ArgumentException("Postgres connection string not defined");
            
            var connBuilder = new NpgsqlConnectionStringBuilder(environmentConnectionString ?? configurationConnectionString)
            {
                PersistSecurityInfo = true,
                Pooling = true,
                CommandTimeout = 15
            };
            
            return connBuilder.ConnectionString;
        }

        public static string BuildRedisConnectionString(IConfiguration configuration)
        {
            var environmentConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
            var configurationConnectionString = configuration.GetConnectionString("Redis");
            
            if (string.IsNullOrWhiteSpace(environmentConnectionString) && string.IsNullOrWhiteSpace(configurationConnectionString))
                throw new ArgumentException("Redis connection string not defined");

            var redisOptions = ConfigurationOptions.Parse(environmentConnectionString ?? configurationConnectionString!);

            redisOptions.AbortOnConnectFail = false;
            redisOptions.ConnectRetry = 5;
            redisOptions.ConnectTimeout = 5000;
            redisOptions.SyncTimeout = 5000;
            redisOptions.AllowAdmin = true;
            redisOptions.DefaultDatabase = 0;
            return redisOptions.ToString();
        }

        public static string BuildRabbitMQConnectionString(IConfiguration configuration)
        {
            var environmentConnectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING");
            var configurationConnectionString = configuration.GetConnectionString("RabbitMQ");
            
            if (string.IsNullOrWhiteSpace(environmentConnectionString) && string.IsNullOrWhiteSpace(configurationConnectionString))
                throw new ArgumentException("RabbitMQ connection string not defined");
            
            return environmentConnectionString ?? configurationConnectionString!;
        }
    }
}
