namespace eGymSystem.API.Middleware;

public sealed class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        logger.LogInformation("HTTP {Method} {Path}", context.Request.Method, context.Request.Path);
        await next(context);
        logger.LogInformation("HTTP {Method} {Path} => {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode);
    }
}
