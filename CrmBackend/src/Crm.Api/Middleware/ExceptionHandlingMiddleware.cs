using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Middleware;

/// <summary>
/// Catches everything that escapes a controller and returns a clean ProblemDetails
/// JSON body. Internal details (stack traces, SQL, provider messages) are logged
/// server-side and never sent to the client.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _log;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> log,
        IHostEnvironment env)
    {
        _next = next;
        _log = log;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client went away — not an error worth reporting.
            _log.LogInformation("Request {Path} was cancelled by the client.", context.Request.Path);
        }
        catch (Exception ex)
        {
            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        // Map known exception types to sensible status codes.
        var (status, title) = ex switch
        {
            KeyNotFoundException      => (StatusCodes.Status404NotFound, "Resource not found"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
            ArgumentException         => (StatusCodes.Status400BadRequest, "Invalid request"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Operation not allowed"),
            _                         => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        // Correlation id so a user-reported error can be found in the logs.
        var traceId = context.TraceIdentifier;

        if (status >= 500)
            _log.LogError(ex, "Unhandled exception on {Method} {Path}. TraceId={TraceId}",
                context.Request.Method, context.Request.Path, traceId);
        else
            _log.LogWarning(ex, "Handled exception on {Method} {Path}. TraceId={TraceId}",
                context.Request.Method, context.Request.Path, traceId);

        if (context.Response.HasStarted)
        {
            _log.LogWarning("Response already started; cannot write error body. TraceId={TraceId}", traceId);
            return;
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Instance = context.Request.Path,
            // Only 4xx get a specific detail. A 500 stays generic in production.
            Detail = status < 500
                ? ex.Message
                : _env.IsDevelopment() ? ex.Message : "Please try again, or contact support with the trace id."
        };
        problem.Extensions["traceId"] = traceId;

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
