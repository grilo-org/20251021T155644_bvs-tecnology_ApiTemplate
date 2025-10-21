using System.Net;
using System.Text.Json;
using API.Middlewares;
using Domain.Common;
using Domain.Common.Constants;
using Domain.Exceptions;
using Domain.SeedWork.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tests.Middlewares;

public class ExceptionMiddlewareTests
{
    private readonly Mock<IHostEnvironment> _mockEnvironment = new();
    private readonly Mock<ILogger<ExceptionMiddleware>> _mockLogger = new();
    private ExceptionMiddleware CreateMiddleware(RequestDelegate next)
    {
        IOptions<JsonOptions> options = Options.Create(new JsonOptions());
        return new ExceptionMiddleware(next, options, _mockEnvironment.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext()
    {
        var context = new DefaultHttpContext();
        var middleware = CreateMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context, Mock.Of<INotification>());

        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ShouldHandleNotificationException()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var notificationMock = new Mock<INotification>();
        notificationMock.Setup(n => n.HasNotification).Returns(true);
        notificationMock.Setup(n => n.Notifications).Returns([
            new NotificationModel("Test notification")
        ]);

        var middleware = CreateMiddleware(_ => throw new NotificationException());
        
        await middleware.InvokeAsync(context, notificationMock.Object);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var response = await reader.ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Contains("Test notification", response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InvokeAsync_ShouldHandleGenericException(bool development)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        _mockEnvironment.Setup(x => x.EnvironmentName).Returns(
            development 
            ? Environments.Development 
            : Environments.Production
        );

        var middleware = CreateMiddleware(_ => throw new Exception("Generic exception"));
        
        await middleware.InvokeAsync(context, Mock.Of<INotification>());

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var response = await reader.ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Contains(
            development 
            ? "Generic exception" 
            : RequestErrorResponseConstant.InternalError,
            response
        );
    }
}