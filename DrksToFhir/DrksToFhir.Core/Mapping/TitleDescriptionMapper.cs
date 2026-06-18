using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class TitleDescriptionMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialDescriptions != null)
        {
            var enDesc = drks.TrialDescriptions.FirstOrDefault(d => d.IdLocale?.Locale == Locale.en);
            var deDesc = drks.TrialDescriptions.FirstOrDefault(d => d.IdLocale?.Locale == Locale.de);

            study.TitleElement = FhirHelper.BuildTranslatedString(enDesc?.Title, deDesc?.Title);

            var acronym = enDesc?.Acronym ?? deDesc?.Acronym;
            if (acronym != null)
            {
                study.Label = [ new ResearchStudy.LabelComponent
                {
                    Type = new CodeableConcept("http://terminology.hl7.org/CodeSystem/title-type", "acronym", "Acronym"),
                    Value = acronym
                }];
            }

            study.DescriptionSummaryElement = FhirHelper.BuildTranslatedMarkdown(enDesc?.Summary, deDesc?.Summary);
            study.DescriptionElement = FhirHelper.BuildTranslatedMarkdown(enDesc?.ScientificSummary, deDesc?.ScientificSummary);
        }
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        var enTitle = study.Title;
        var deTitle = GetTranslation(study.TitleElement);
        var enSummary = study.DescriptionSummary;
        var deSummary = GetTranslation(study.DescriptionSummaryElement);
        var enScientific = study.Description;
        var deScientific = GetTranslation(study.DescriptionElement);
        var acronym = study.Label.FirstOrDefault(l => l.Type?.Coding.Any(c => c.Code == "acronym") == true)?.Value;

        var descriptions = new List<TrialDescription>();

        if (enTitle != null || enSummary != null || enScientific != null)
        {
            descriptions.Add(new TrialDescription
            {
                IdLocale = new IdLocale { Locale = Locale.en },
                Title = enTitle,
                Acronym = acronym,
                Summary = enSummary,
                ScientificSummary = enScientific
            });
        }

        if (deTitle != null || deSummary != null || deScientific != null)
        {
            descriptions.Add(new TrialDescription
            {
                IdLocale = new IdLocale { Locale = Locale.de },
                Title = deTitle,
                Acronym = acronym,
                Summary = deSummary,
                ScientificSummary = deScientific
            });
        }            

        drks.TrialDescriptions = descriptions.Count > 0 ? descriptions : null;
    }

    internal static string? GetTranslation(PrimitiveType? element)
    {
        if (element == null)
        {
            return null;
        }
        var ext = element.Extension.FirstOrDefault(e => e.Url == "http://hl7.org/fhir/StructureDefinition/translation");
        return (ext?.Extension.FirstOrDefault(e => e.Url == "content")?.Value as FhirString)?.Value;
    }
}