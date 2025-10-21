using Domain.Entities.Dtos;

namespace Domain.Interfaces.Services;

public interface ITestService
{
    Task<TestConsumerDto> TestExchange();
}