using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ObservationalGroupMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.ComparisonGroup = [];

        if (drks.ObservationalGroups != null)
        {
            var grouped = drks.ObservationalGroups.GroupBy(g => g.IdArm?.Arm).ToList();

            foreach (var grp in grouped)
            {
                var enComment = grp.FirstOrDefault(g => g.IdArm?.Locale == Locale.en)?.Comment;
                var deComment = grp.FirstOrDefault(g => g.IdArm?.Locale == Locale.de)?.Comment;

                study.ComparisonGroup.Add(new ResearchStudy.ComparisonGroupComponent
                {
                    LinkId = grp.Key?.ToString() ?? "1",
                    Name = "observational group",
                    Type = new CodeableConcept(
                        "http://hl7.org/fhir/research-study-arm-type", "experimental", "Experimental"),
                    DescriptionElement = FhirHelper.BuildTranslatedMarkdown(enComment, deComment)
                });
            }
        }

        if (drks.StudyCharacteristic?.ControlTypes != null)
        {
            foreach (var item in drks.StudyCharacteristic.ControlTypes)
            {
                var (system, code, display) = item.IdControlType?.ControlType switch
                {
                    ControlType.UNCONTROLLED_SINGLE_ARM => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/ArmType", "uncontrolled-single-arm-comparator", "Uncontrolled single arm comparator"),
                    ControlType.PLACEBO => ("http://hl7.org/fhir/research-study-arm-type", "placebo-comparator", "Placebo comparator"),
                    ControlType.ACTIVE_CONTROL => ("http://hl7.org/fhir/research-study-arm-type", "active-comparator", "Active comparator"),
                    ControlType.HISTORICAL => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/ArmType", "historical-comparator", "Historical comparator"),
                    ControlType.RECEIVES_NO_TREATMENT => ("http://hl7.org/fhir/research-study-arm-type", "no-intervention", "No intervention"),
                    ControlType.OTHER => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/ArmType", "other-comparator", "Other comparator"),
                    _ => (null, null, null)
                };

                if (code != null)
                {
                    study.ComparisonGroup.Add(new ResearchStudy.ComparisonGroupComponent
                    {
                        Name = "control group",
                        Type = new CodeableConcept(system, code, display)
                    });
                }
            }
        }
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        if (study.ComparisonGroup.Count == 0) return;

        var groups = new List<ObservationalGroup>();
        var observationalGroups = study.ComparisonGroup.Where(cg => cg.Name == "observational group");

        foreach (var cg in observationalGroups)
        {
            if (int.TryParse(cg.LinkId, out var armNum))
            {
                var enDesc = cg.Description;
                var deDesc = TitleDescriptionMapper.GetTranslation(cg.DescriptionElement);

                if (enDesc != null)
                {
                    groups.Add(new ObservationalGroup
                    {
                        IdArm = new IdArm { Arm = armNum, Locale = Locale.en },
                        Comment = enDesc
                    });
                }
                if (deDesc != null)
                {
                    groups.Add(new ObservationalGroup
                    {
                        IdArm = new IdArm { Arm = armNum, Locale = Locale.de },
                        Comment = deDesc
                    });
                }
            }
        }

        drks.ObservationalGroups = groups.Count > 0 ? groups : null;

        var controlGroups = study.ComparisonGroup.Where(cg => cg.Name == "control group").ToList();
        if (controlGroups.Count > 0)
        {
            var controlTypes = new List<ControlTypeItem>();

            foreach (var cg in controlGroups)
            {
                var code = cg.Type?.Coding.FirstOrDefault()?.Code;
                ControlType? ct = code switch
                {
                    "uncontrolled-single-arm-comparator" => ControlType.UNCONTROLLED_SINGLE_ARM,
                    "placebo-comparator" => ControlType.PLACEBO,
                    "active-comparator" => ControlType.ACTIVE_CONTROL,
                    "historical-comparator" => ControlType.HISTORICAL,
                    "no-intervention" => ControlType.RECEIVES_NO_TREATMENT,
                    "other-comparator" => ControlType.OTHER,
                    _ => null
                };

                if (ct.HasValue)
                {
                    controlTypes.Add(new ControlTypeItem
                    {
                        IdControlType = new IdControlType { ControlType = ct.Value }
                    });
                }
            }

            if (controlTypes.Count > 0)
            {
                drks.StudyCharacteristic ??= new StudyCharacteristic();
                drks.StudyCharacteristic.ControlTypes = controlTypes;
            }
        }
    }
}