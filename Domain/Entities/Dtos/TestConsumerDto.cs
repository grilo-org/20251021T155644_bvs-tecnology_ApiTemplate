namespace Domain.Entities.Dtos;

public class TestConsumerDto : BaseEntity
{
    public TestConsumerDto(): base(Guid.CreateVersion7()) {}
    public TestConsumerDto(Guid id): base(id) {}
}