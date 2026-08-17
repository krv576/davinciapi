using DavinciEPA.Core.Interfaces.Rules;
using DavinciEPA.Rules.Coverage;
using DavinciEPA.Rules.Documentation;
using DavinciEPA.Rules.MedicalNecessity;
using DavinciEPA.Rules.PriorAuthorization;
using Microsoft.Extensions.DependencyInjection;

namespace DavinciEPA.Rules.Extensions;

/// <summary>Registers the coverage, medical necessity, prior authorization, and questionnaire pre-population rule engines.</summary>
public static class RulesServiceCollectionExtensions
{
    public static IServiceCollection AddDavinciRules(this IServiceCollection services)
    {
        services.AddSingleton<ICoverageRuleEngine, CoverageRuleEngine>();
        services.AddSingleton<IMedicalNecessityRuleEngine, MedicalNecessityRuleEngine>();
        services.AddSingleton<IPriorAuthorizationRuleEngine, PriorAuthorizationRuleEngine>();
        services.AddSingleton<IQuestionnairePrePopulationEngine, QuestionnairePrePopulationEngine>();

        return services;
    }
}
