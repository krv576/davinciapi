using DavinciEPA.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DavinciEPA.Shared.Middleware;

/// <summary>A translated exception outcome ready to be written as an HTTP <see cref="ProblemDetails"/> response.</summary>
public sealed record ExceptionMappingResult(int StatusCode, string Title, string Detail);

/// <summary>
/// Translates a caught exception into an <see cref="ExceptionMappingResult"/>. Implemented outside
/// <c>DavinciEPA.Shared</c> (e.g. in <c>DavinciEPA.Core</c>) so this project never needs to know about
/// domain-specific exception types.
/// </summary>
public interface IExceptionResponseMapper
{
    bool TryMap(Exception exception, out ExceptionMappingResult result);
}

/// <summary>Global exception handling middleware: logs the failure (without PHI) and returns a consistent <see cref="ProblemDetails"/> response.</summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IExceptionResponseMapper? _mapper;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IExceptionResponseMapper? mapper = null)
    {
        _next = next;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.GetCorrelationId();

            ExceptionMappingResult mapped;
            if (_mapper is not null && _mapper.TryMap(ex, out var mappingResult))
            {
                mapped = mappingResult;
            }
            else
            {
                mapped = new ExceptionMappingResult(
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    "The server encountered an unexpected condition and could not complete the request.");
            }

            if (mapped.StatusCode >= 500)
            {
                _logger.LogError(ex, "Unhandled exception. CorrelationId={CorrelationId}", correlationId);
            }
            else
            {
                _logger.LogWarning(
                    "Request failed with {StatusCode}: {Title}. CorrelationId={CorrelationId}",
                    mapped.StatusCode,
                    mapped.Title,
                    correlationId);
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = mapped.StatusCode;

            var problem = new ProblemDetails
            {
                Status = mapped.StatusCode,
                Title = mapped.Title,
                Detail = mapped.Detail,
                Instance = context.Request.Path
            };
            problem.Extensions["correlationId"] = correlationId;

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}

/// <summary>Registers the Shared middleware pipeline components.</summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationIdMiddleware>();

    public static IApplicationBuilder UseDavinciExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
