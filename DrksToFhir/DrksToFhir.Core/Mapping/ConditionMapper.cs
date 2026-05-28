using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ConditionMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.StudiedHealthConditions is null) return;

        study.Condition = [];

        foreach (var condition in drks.StudiedHealthConditions)
        {
            var enText = condition.StudiedHealthConditionDescriptions?
                .FirstOrDefault(d => d.IdLocale.Locale == Locale.en)?.Comment;
            var deText = condition.StudiedHealthConditionDescriptions?
                .FirstOrDefault(d => d.IdLocale.Locale == Locale.de)?.Comment;

            Coding? coding = null;
            if (condition.Catalog == HealthConditionCatalog.ICD10 && condition.Code is not null)
                coding = new Coding("http://hl7.org/fhir/sid/icd-10", condition.Code);
            else if (condition.Catalog == HealthConditionCatalog.ORPHA_CODE && condition.Code is not null)
                coding = new Coding("https://www.orpha.net", condition.Code);

            var cc = new CodeableConcept();
            if (coding is not null)
                cc.Coding = [coding];
            cc.TextElement = FhirHelper.BuildTranslatedString(enText, deText);

            cc.Extension.Add(new Extension(
                "https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-condition-type",
                new Code(condition.Type?.ToString() ?? "OTHER")));

            study.Condition.Add(cc);
        }
    }
}