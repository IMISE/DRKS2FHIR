using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ObservationalGroupMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.ObservationalGroups is null) return;

        study.ComparisonGroup = [];

        var grouped = drks.ObservationalGroups
            .GroupBy(g => g.IdArm.Arm)
            .ToList();

        foreach (var grp in grouped)
        {
            var enComment = grp.FirstOrDefault(g => g.IdArm.Locale == Locale.en)?.Comment;
            var deComment = grp.FirstOrDefault(g => g.IdArm.Locale == Locale.de)?.Comment;

            study.ComparisonGroup.Add(new ResearchStudy.ComparisonGroupComponent
            {
                LinkId = grp.Key?.ToString() ?? "1",
                Name = "observational group",
                DescriptionElement = FhirHelper.BuildTranslatedMarkdown(enComment, deComment)
            });
        }
    }
}