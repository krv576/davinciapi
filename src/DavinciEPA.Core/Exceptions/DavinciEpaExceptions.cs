namespace DavinciEPA.Core.Exceptions;

/// <summary>Base type for all domain/application exceptions raised by this platform.</summary>
public abstract class DavinciEpaException : Exception
{
    protected DavinciEpaException(string message) : base(message)
    {
    }

    protected DavinciEpaException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>Thrown when a requested entity (prior authorization request, requirement, etc.) does not exist.</summary>
public sealed class EntityNotFoundException : DavinciEpaException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"{entityName} with identifier '{key}' was not found.")
    {
        EntityName = entityName;
        Key = key;
    }

    public string EntityName { get; }
    public object Key { get; }
}

/// <summary>Thrown when a domain invariant or business rule is violated by the requested operation.</summary>
public sealed class DomainValidationException : DavinciEpaException
{
    public DomainValidationException(string message) : base(message)
    {
        Failures = Array.Empty<string>();
    }

    public DomainValidationException(IReadOnlyCollection<string> failures)
        : base("One or more domain validation rules were violated.")
    {
        Failures = failures;
    }

    public IReadOnlyCollection<string> Failures { get; }
}

/// <summary>Thrown when a FHIR resource fails structural or profile conformance validation.</summary>
public sealed class FhirValidationException : DavinciEpaException
{
    public FhirValidationException(string resourceType, IReadOnlyCollection<string> issues)
        : base($"{resourceType} failed FHIR profile validation with {issues.Count} issue(s).")
    {
        ResourceType = resourceType;
        Issues = issues;
    }

    public string ResourceType { get; }
    public IReadOnlyCollection<string> Issues { get; }
}

/// <summary>Thrown when a call to an external system (payer, EHR FHIR server, identity provider) fails.</summary>
public sealed class ExternalServiceException : DavinciEpaException
{
    public ExternalServiceException(string serviceName, string message) : base(message)
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, string message, Exception innerException)
        : base(message, innerException)
    {
        ServiceName = serviceName;
    }

    public string ServiceName { get; }
}
