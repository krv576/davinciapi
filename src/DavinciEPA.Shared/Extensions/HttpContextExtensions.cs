using DavinciEPA.Shared.Middleware;

namespace DavinciEPA.Shared.Extensions;

/// <summary>Convenience accessors attached to <see cref="HttpContext"/> for cross-cutting request data.</summary>
public static class HttpContextExtensions
{
    public static string GetCorrelationId(this HttpContext context) =>
        context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value) && value is string correlationId
            ? correlationId
            : string.Empty;
}
