using DavinciEPA.Shared.Middleware;

namespace DavinciEPA.Core.Exceptions;

/// <summary>
/// Maps <see cref="DavinciEpaException"/> subtypes to HTTP status codes for the Shared exception-handling
/// middleware. Lives in Core (rather than Shared) because Shared must not depend on domain exception types.
/// </summary>
public sealed class CoreExceptionResponseMapper : IExceptionResponseMapper
{
    public bool TryMap(Exception exception, out ExceptionMappingResult result)
    {
        // Status codes are expressed as literals (400/404/422/500/502) rather than Microsoft.AspNetCore.Http.StatusCodes
        // so that DavinciEPA.Core carries no dependency on ASP.NET Core.
        result = exception switch
        {
            EntityNotFoundException notFound => new ExceptionMappingResult(
                404, "Resource not found.", notFound.Message),

            DomainValidationException validation => new ExceptionMappingResult(
                400, "Validation failed.", validation.Message),

            FhirValidationException fhirValidation => new ExceptionMappingResult(
                422, "FHIR resource validation failed.", fhirValidation.Message),

            ExternalServiceException external => new ExceptionMappingResult(
                502, $"Upstream service '{external.ServiceName}' failed.", external.Message),

            DavinciEpaException generic => new ExceptionMappingResult(
                400, "Request could not be processed.", generic.Message),

            _ => new ExceptionMappingResult(
                500, "An unexpected error occurred.", "The server encountered an unexpected condition.")
        };

        return exception is DavinciEpaException;
    }
}
