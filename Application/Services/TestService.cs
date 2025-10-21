using Domain.Entities.Dtos;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using MassTransit;

namespace Application.Services;

internal class TestService(ILogger<TestService> logger, IBus bus) : ITestService
{
    public async Task<TestConsumerDto> TestExchange()
    {
        var testDto = new TestConsumerDto(Guid.CreateVersion7());
        logger.LogInformation("Sending test {Id} to RabbitMQ", testDto.Id);
        await bus.Publish(testDto);
        logger.LogInformation("Test {Id} sent to RabbitMQ", testDto.Id);
        return testDto;
    }
}