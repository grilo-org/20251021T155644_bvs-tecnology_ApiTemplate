using System.Net;
using Domain.Common;
using Domain.SeedWork.Notification;
using System.Text.Json;
using Domain.Common.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Middlewares
{
    public class ExceptionMiddleware(
        RequestDelegate next,
        IOptions<JsonOptions> options,
        IHostEnvironment environment,
        ILogger<ExceptionMiddleware> logger
    )
    {
        private readonly JsonSerializerOptions _jsonOptions = options.Value.JsonSerializerOptions;
        public async Task InvokeAsync(HttpContext context, INotification notification)
        {
            try
            {
                await next(context);
            }
            catch (NotificationException ex)
            {
                logger.LogWarning(ex, "{Message} | Path: {Path}", ExceptionMessage.MappedException, context.Request.Path);
                await HandleExceptionAsync(context, notification);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message} | Path: {Path}", ExceptionMessage.UnexpectedException, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, INotification notification)
        {
            var result = ErrorResponse.Factory.Create(notification.Notifications);
            UpdateContext(context, HttpStatusCode.InternalServerError);
            var stringResponse = JsonSerializer.Serialize(result, _jsonOptions);
            await context.Response.WriteAsync(stringResponse);
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = ErrorResponse.Factory.Create(
                environment.IsDevelopment() 
                ? exception.ToString() 
                : RequestErrorResponseConstant.InternalError
            );
            UpdateContext(context, HttpStatusCode.InternalServerError);
            var stringResponse = JsonSerializer.Serialize(result, _jsonOptions);
            await context.Response.WriteAsync(stringResponse);
        }
        private static void UpdateContext(HttpContext context, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
        }
    }
}
