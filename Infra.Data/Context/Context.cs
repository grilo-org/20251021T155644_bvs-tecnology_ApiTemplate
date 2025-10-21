using System.Diagnostics.CodeAnalysis;
using Domain.Entities.Dtos;
using Infra.Data.Context.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Context
{
    [ExcludeFromCodeCoverage]
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public virtual DbSet<TestConsumerDto> TestConsumerDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("template-api");
            modelBuilder.ApplyConfiguration(new TestConfigurations());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
