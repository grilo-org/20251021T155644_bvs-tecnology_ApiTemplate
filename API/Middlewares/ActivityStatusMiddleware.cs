using System.Diagnostics;

namespace API.Middlewares;

public class ActivityStatusMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;
        try
        {
            await next(context);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }
}