using Domain.Entities.Dtos;

namespace Tests.Mocks.Entities.Dtos;

public class TestConsumerDtoMock : BaseMock<TestConsumerDto>
{
    protected override TestConsumerDto GetEntity(Guid id) => new TestConsumerDto(id);
}