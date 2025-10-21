using Domain.Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Context.Configurations;

public class TestConfigurations : IEntityTypeConfiguration<TestConsumerDto>
{
    public void Configure(EntityTypeBuilder<TestConsumerDto> builder)
    {
        builder.ToTable("TestConsumerDtos");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
    }
}