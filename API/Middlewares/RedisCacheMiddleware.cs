using System.Text;
using Domain.Common.Constants;
using Microsoft.Extensions.Caching.Distributed;
using static System.String;

namespace API.Middlewares;
public class RedisCacheMiddleware(RequestDelegate next, IDistributedCache distributedCache)
{
    private string _requestBody = Empty;
    public async Task InvokeAsync(HttpContext context)
    {
        if (NotAllowedCache(context))
        {
            await next(context);
            return;
        }

        _requestBody = await GetRequestBody(context.Request);
        var cacheKey = GenerateCacheKeyFromRequest(context);
        var cachedResponse = await distributedCache.GetStringAsync(cacheKey);
        if (!IsNullOrEmpty(cachedResponse))
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        var originalBodyStream = context.Response.Body;
        await using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await next(context);
        }
        catch (Exception)
        {
            // Não fecha a stream automaticamente em caso de exceptions.
            context.Response.Body = originalBodyStream;
            throw;
        }
        
        context.Response.Body = originalBodyStream;
        
        if (context.Response.StatusCode == StatusCodes.Status200OK)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };
            await distributedCache.SetStringAsync(cacheKey, responseBody, options);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBodyStream);
    }
    private static async Task<string> GetRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private string GenerateCacheKeyFromRequest(HttpContext context)
    {
        var request = context.Request;
        var keyBuilder = new StringBuilder();

        keyBuilder.Append($"{request.Method}:{request.Path}");

        if (request.QueryString.HasValue)
        {
            keyBuilder.Append($"?{request.QueryString.Value}");
        }

        if (request.Method is RequestMethod.Post or RequestMethod.Put)
        {
            keyBuilder.Append($":{_requestBody}");
        }

        return keyBuilder.ToString();
    }

    private static bool NotAllowedCache(HttpContext context)
    {
        var notAllowedRequests = new List<(string Path, string Method)>
        {
            ("/api/fake-path", RequestMethod.Post)
        };

        var requestPath = context.Request.Path.ToString().ToLower();
        var requestMethod = context.Request.Method.ToUpper();

        return notAllowedRequests.Any(r => r.Path == requestPath && r.Method == requestMethod);
    }
}