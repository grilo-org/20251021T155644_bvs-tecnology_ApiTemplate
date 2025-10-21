using System.Diagnostics;
using API.Middlewares;
using Microsoft.AspNetCore.Http;

namespace Tests.Middlewares;

public class ActivityStatusMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNoException_SetsActivityStatusOk()
    {
        var context = new DefaultHttpContext();
        var middleware = new ActivityStatusMiddleware(_ => Task.CompletedTask);
        using var activity = new Activity("TestActivity").Start();

        await middleware.InvokeAsync(context);

        Assert.Equal(ActivityStatusCode.Ok, activity.Status);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_SetsActivityStatusErrorAndThrows()
    {
        var context = new DefaultHttpContext();
        var middleware = new ActivityStatusMiddleware(_ => throw new InvalidOperationException("boom"));

        using var activity = new Activity("TestActivity").Start();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));
        Assert.Equal("boom", exception.Message);
        Assert.Equal(ActivityStatusCode.Error, activity.Status);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoActivity_DoesNotThrow()
    {
        var context = new DefaultHttpContext();
        var middleware = new ActivityStatusMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        
        Assert.Null(Activity.Current);
    }
}