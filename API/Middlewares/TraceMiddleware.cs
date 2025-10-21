using System.Diagnostics;

namespace API.Middlewares;

public class TraceMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;
        if (activity != null) AppendTraceIdToHeaders(context, activity);
        await next(context);
    }
    
    private static void AppendTraceIdToHeaders(HttpContext context, Activity activity)
    {
        var traceId = activity.TraceId.ToString();
        if (!string.IsNullOrEmpty(traceId) && !context.Response.Headers.ContainsKey("X-Trace-Id"))
            context.Response.Headers.Append("X-Trace-Id", traceId);
    }
}