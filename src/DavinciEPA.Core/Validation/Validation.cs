namespace DavinciEPA.Core.Validation;

/// <summary>A single field-level validation failure.</summary>
public sealed record ValidationFailure(string PropertyName, string ErrorMessage);

/// <summary>The outcome of validating an input model, independent of FHIR/HTTP concerns.</summary>
public sealed class ValidationResult
{
    private ValidationResult(IReadOnlyCollection<ValidationFailure> failures)
    {
        Failures = failures;
    }

    public IReadOnlyCollection<ValidationFailure> Failures { get; }
    public bool IsValid => Failures.Count == 0;

    public static ValidationResult Success() => new(Array.Empty<ValidationFailure>());

    public static ValidationResult Failed(params ValidationFailure[] failures) => new(failures);

    public static ValidationResult Failed(IReadOnlyCollection<ValidationFailure> failures) => new(failures);
}

/// <summary>Contract for synchronous validation of application-layer input models.</summary>
public interface IValidator<in T>
{
    ValidationResult Validate(T instance);
}
