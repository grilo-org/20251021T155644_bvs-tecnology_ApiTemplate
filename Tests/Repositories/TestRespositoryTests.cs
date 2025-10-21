using Domain.Entities.Dtos;
using Infra.Data.Repositories;
using Tests.Mocks.Entities.Dtos;

namespace Tests.Repositories;

public class TestRespositoryTests : BaseRepositoryTest<TestRepository, TestConsumerDtoMock, TestConsumerDto>;