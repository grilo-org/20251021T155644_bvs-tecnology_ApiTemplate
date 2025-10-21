using System.Text;
using API.Middlewares;
using Domain.Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Tests.Middlewares;

public class RedisCacheMiddlewareTests
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly DefaultHttpContext _context;

    public RedisCacheMiddlewareTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    private static RequestDelegate BuildRequestDelegate(string responseContent, bool throwException = false)
    {
        return async ctx =>
        {
            if (throwException) throw new InvalidOperationException("Test exception");

            ctx.Response.StatusCode = StatusCodes.Status200OK;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(responseContent);
        };
    }

    [Fact]
    public async Task InvokeAsync_WhenExistCache_ShouldReturnCachedResponse()
    {
        // Arrange
        var expectedResponse = "{\"message\":\"cached response\"}";
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Encoding.UTF8.GetBytes(expectedResponse));

        var middleware = new RedisCacheMiddleware(BuildRequestDelegate("não deve chamar"), _cacheMock.Object);
        _context.Request.Method = RequestMethod.Get;
        _context.Request.Path = "/api/test";

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();

        Assert.Equal(expectedResponse, body);
        _cacheMock.Verify(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotExistCache_ShouldSaveResponseOnCache()
    {
        // Arrange
        var expectedResponse = "{\"message\":\"new response\"}";
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((byte[]?)null);

        var middleware = new RedisCacheMiddleware(BuildRequestDelegate(expectedResponse), _cacheMock.Object);
        _context.Request.Method = RequestMethod.Put;
        _context.Request.Path = "/api/test";

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();

        Assert.Equal(expectedResponse, body);
        _cacheMock.Verify(c => c
            .SetAsync(
                It.IsAny<string>(),
                It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == expectedResponse),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotAllowedToCache_ShouldNotGetCachedResponse()
    {
        // Arrange
        var expectedResponse = "{\"message\":\"cached response\"}";
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Encoding.UTF8.GetBytes(expectedResponse));

        var middleware = new RedisCacheMiddleware(BuildRequestDelegate("{\"message\":\"from next\"}"), _cacheMock.Object);
        _context.Request.Method = RequestMethod.Post;
        _context.Request.Path = "/api/fake-path";

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(_context.Response.Body).ReadToEndAsync();

        Assert.Contains("from next", body);
        _cacheMock.Verify(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WhenException_ShouldRestoreOriginalResponse()
    {
        // Arrange
        var middleware = new RedisCacheMiddleware(BuildRequestDelegate("ignored", throwException: true), _cacheMock.Object);
        _context.Request.Method = RequestMethod.Get;
        _context.Request.Path = "/api/test";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(_context));

        // Response.Body deve ser o original
        Assert.IsType<MemoryStream>(_context.Response.Body);
    }
}