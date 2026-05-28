using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class TitleDescriptionMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialDescriptions is null) return;

        var enDesc = drks.TrialDescriptions.FirstOrDefault(d => d.IdLocale.Locale == Locale.en);
        var deDesc = drks.TrialDescriptions.FirstOrDefault(d => d.IdLocale.Locale == Locale.de);

        study.TitleElement = FhirHelper.BuildTranslatedString(enDesc?.Title, deDesc?.Title);

        var acronym = enDesc?.Acronym ?? deDesc?.Acronym;
        if (acronym is not null)
        {
            study.Label =
            [
                new ResearchStudy.LabelComponent
                {
                    Type = new CodeableConcept(
                        "http://terminology.hl7.org/CodeSystem/title-type",
                        "acronym", "Acronym"),
                    Value = acronym
                }
            ];
        }

        study.DescriptionSummaryElement =
            FhirHelper.BuildTranslatedMarkdown(enDesc?.Summary, deDesc?.Summary);

        study.DescriptionElement =
            FhirHelper.BuildTranslatedMarkdown(enDesc?.ScientificSummary, deDesc?.ScientificSummary);
    }
}