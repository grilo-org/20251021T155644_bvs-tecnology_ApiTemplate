using Application.Services;
using Domain.Entities.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Tests.Services;

public class TestServiceTests
{
    private readonly Mock<IBus> _bus = new();
    private readonly Mock<ILogger<TestService>> _logger = new();
    private readonly TestService _service;

    public TestServiceTests()
        => _service = new TestService(_logger.Object, _bus.Object);

    [Fact]
    public async Task ShouldSendModelToExchange()
    {
        var result = await _service.TestExchange();
        
        Assert.NotNull(result);
        Assert.IsType<TestConsumerDto>(result);
        _bus.Verify(x => x.Publish(It.IsAny<TestConsumerDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}