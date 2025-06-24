using Serilog;
using System.Diagnostics;
using System.Security.Claims;

namespace PixelMartShop.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger = Log.ForContext<RequestLoggingMiddleware>();

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var request = context.Request;
        var user = context.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "unknown";
        var roles = string.Join(",", user.FindAll(ClaimTypes.Role).Select(r => r.Value));

        var path = request.Path;
        var method = request.Method;
        var ip = context.Connection.RemoteIpAddress?.ToString();

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;

        _logger.Information(
            "Request {Method} {Path} by UserId={UserId}, Email={Email}, Roles={Roles}, IP={IP}, Status={StatusCode}, Duration={Elapsed}ms",
            method, path, userId, email, roles, ip, statusCode, stopwatch.ElapsedMilliseconds);
    }
}
