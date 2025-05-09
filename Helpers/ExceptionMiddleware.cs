﻿namespace PixelMartShop.Helpers;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = _env.IsDevelopment() ? ex.Message : "Internal Server Error",
                Details = _env.IsDevelopment() ? ex.StackTrace : null
            };

            var json = System.Text.Json.JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
