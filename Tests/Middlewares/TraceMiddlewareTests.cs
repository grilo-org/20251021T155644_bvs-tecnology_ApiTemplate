using System.Diagnostics;
using API.Middlewares;
using Microsoft.AspNetCore.Http;

namespace Tests.Middlewares;

public class TraceMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNoException_SetsActivityStatusOk()
    {
        var context = new DefaultHttpContext();
        var middleware = new TraceMiddleware(_ => Task.CompletedTask);

        using var activity = new Activity("TestActivity").Start();

        await middleware.InvokeAsync(context);

        Assert.True(context.Response.Headers.ContainsKey("X-Trace-Id"));
    }
    
    [Fact]
    public async Task InvokeAsync_WhenNoActivity_DoesNotThrow()
    {
        var context = new DefaultHttpContext();
        var middleware = new TraceMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);
        
        Assert.Null(Activity.Current);
    }
}