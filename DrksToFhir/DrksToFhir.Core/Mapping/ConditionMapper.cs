using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ConditionMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.StudiedHealthConditions != null)
        {
            study.Condition = [];

            foreach (var condition in drks.StudiedHealthConditions)
            {
                var enText = condition.StudiedHealthConditionDescriptions?.FirstOrDefault(d => d.IdLocale?.Locale == Locale.en)?.Comment;
                var deText = condition.StudiedHealthConditionDescriptions?.FirstOrDefault(d => d.IdLocale?.Locale == Locale.de)?.Comment;

                Coding? coding = null;
                if (condition.Catalog == HealthConditionCatalog.ICD10 && condition.Code != null)
                {
                    coding = new Coding("http://hl7.org/fhir/sid/icd-10", condition.Code);
                }                    
                else if (condition.Catalog == HealthConditionCatalog.ORPHA_CODE && condition.Code != null)
                {
                    coding = new Coding("https://www.orpha.net", condition.Code);
                }

                var cc = new CodeableConcept();
                if (coding != null)
                {
                    cc.Coding = [coding];
                }                    
                cc.TextElement = FhirHelper.BuildTranslatedString(enText, deText);

                cc.Extension.Add(new Extension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-is-main-condition", new FhirBoolean(condition.Type == HealthConditionType.MAIN)));

                study.Condition.Add(cc);
            }
        }
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        if (study.Condition.Count > 0)
        {
            drks.StudiedHealthConditions = study.Condition.Select(c =>
            {
                var isMain = (c.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-is-main-condition")?.Value as FhirBoolean)?.Value ?? false;

                var coding = c.Coding.FirstOrDefault();
                var enText = c.Text;
                var deText = TitleDescriptionMapper.GetTranslation(c.TextElement);

                HealthConditionCatalog? catalog = coding?.System switch
                {
                    "http://hl7.org/fhir/sid/icd-10" => HealthConditionCatalog.ICD10,
                    "https://www.orpha.net" => HealthConditionCatalog.ORPHA_CODE,
                    _ => HealthConditionCatalog.FREETEXT
                };

                var descriptions = new List<StudiedHealthConditionDescription>();
                if (enText != null)
                {
                    descriptions.Add(new StudiedHealthConditionDescription
                    {
                        IdLocale = new IdLocale { Locale = Locale.en },
                        Comment = enText
                    });
                }
                if (deText != null)
                {
                    descriptions.Add(new StudiedHealthConditionDescription
                    {
                        IdLocale = new IdLocale { Locale = Locale.de },
                        Comment = deText
                    });
                }                   

                return new StudiedHealthCondition
                {
                    Code = coding?.Code,
                    Catalog = catalog,
                    Type = isMain ? HealthConditionType.MAIN : HealthConditionType.OTHER,
                    StudiedHealthConditionDescriptions = descriptions.Count > 0 ? descriptions : null
                };
            }).ToList();
        }
    }
}