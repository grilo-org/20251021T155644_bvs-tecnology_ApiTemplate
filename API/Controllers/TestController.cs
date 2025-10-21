using Domain.Common;
using Domain.Entities.Dtos;
using Domain.Exceptions;
using Domain.Interfaces.Services;
using Domain.SeedWork.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TestController(
    ILogger<TestController> logger,
    ITestService testService,
    INotification notification
) : BaseController
{
    [HttpGet("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public IActionResult Token()
    {
        return Ok();
    }
    
    [HttpGet("authorize")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public IActionResult Authorize()
    {
        return Ok();
    }

    [HttpGet("free")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TestConsumerDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Free()
    {
        logger.LogInformation("starting free method");
        var result = await testService.TestExchange();
        logger.LogInformation("finishing free method");
        return Ok(result);
    }

    [HttpGet("free/random-error")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public IActionResult RandomError()
    {
        logger.LogInformation("starting random error method");
        throw new ArgumentException("error while trying to resolve random error method");
    }

    [HttpGet("free/notification-error")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public IActionResult NotificationError()
    {
        logger.LogInformation("starting notification error method");
        notification.AddNotification("error while trying to resolve notification error method");
        throw new NotificationException();
    }
}