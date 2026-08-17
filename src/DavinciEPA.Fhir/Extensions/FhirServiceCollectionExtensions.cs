using DavinciEPA.Core.Interfaces.Fhir;
using DavinciEPA.Fhir.Builders;
using DavinciEPA.Fhir.Mapping;
using DavinciEPA.Fhir.Serialization;
using DavinciEPA.Fhir.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace DavinciEPA.Fhir.Extensions;

/// <summary>Registers the FHIR serialization, validation, builder, and mapping services for DI.</summary>
public static class FhirServiceCollectionExtensions
{
    public static IServiceCollection AddDavinciFhir(this IServiceCollection services)
    {
        services.AddSingleton<FhirJsonSerializerService>();
        services.AddSingleton<IOperationOutcomeBuilder, OperationOutcomeBuilder>();
        services.AddSingleton<IFhirResourceValidator, FhirResourceValidator>();
        services.AddSingleton<IClaimBuilder, ClaimBuilder>();
        services.AddSingleton<IClaimResponseBuilder, ClaimResponseBuilder>();
        services.AddSingleton<IQuestionnaireBuilder, QuestionnaireBuilder>();
        services.AddSingleton<IBundleBuilder, BundleBuilder>();
        services.AddSingleton<PasBundleExtractor>();
        services.AddSingleton<QuestionnaireResponseExtractor>();
        services.AddSingleton<OrderCodeExtractor>();
        services.AddSingleton<PrefetchResourceInspector>();
        services.AddSingleton<CoveragePayerExtractor>();

        return services;
    }
}
