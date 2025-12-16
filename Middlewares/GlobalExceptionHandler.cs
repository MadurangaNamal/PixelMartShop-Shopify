using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace PixelMartShop.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private const string CONTENT_TYPE = "application/json";

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

            context.Response.StatusCode = 500;
            context.Response.ContentType = CONTENT_TYPE;

            var errorResponse = new ProblemDetails
            {
                Status = 500,
                Type = ex.GetType().Name,
                Title = ex.Message,
                Detail = "An unexpected error occurred. Please try again later."
            };

            if (!context.Response.HasStarted)
            {
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                Log.Error(ex, "Unhandled exception occurred.");
            }
        }
    }
}

public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandler>();
    }
}