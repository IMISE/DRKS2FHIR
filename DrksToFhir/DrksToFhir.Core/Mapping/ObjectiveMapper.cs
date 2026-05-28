using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ObjectiveMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.StudyCharacteristic?.CharacteristicDescriptions is null) return;

        study.Objective = [];

        var enDesc = drks.StudyCharacteristic.CharacteristicDescriptions
            .FirstOrDefault(d => d.IdLocale.Locale == Locale.en);
        var deDesc = drks.StudyCharacteristic.CharacteristicDescriptions
            .FirstOrDefault(d => d.IdLocale.Locale == Locale.de);

        if (enDesc?.PrimaryOutcome is not null || deDesc?.PrimaryOutcome is not null)
        {
            study.Objective.Add(new ResearchStudy.ObjectiveComponent
            {
                Name = "primary",
                DescriptionElement = FhirHelper.BuildTranslatedMarkdown(
                    enDesc?.PrimaryOutcome, deDesc?.PrimaryOutcome)
            });
        }

        if (enDesc?.SecondaryOutcome is not null || deDesc?.SecondaryOutcome is not null)
        {
            study.Objective.Add(new ResearchStudy.ObjectiveComponent
            {
                Name = "secondary",
                DescriptionElement = FhirHelper.BuildTranslatedMarkdown(
                    enDesc?.SecondaryOutcome, deDesc?.SecondaryOutcome)
            });
        }
    }
}