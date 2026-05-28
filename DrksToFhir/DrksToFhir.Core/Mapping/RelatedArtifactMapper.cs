using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class RelatedArtifactMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialResults?.Publications is null) return;

        study.RelatedArtifact = [];

        foreach (var pub in drks.TrialResults.Publications)
        {
            var (code, display) = pub.Type switch
            {
                PublicationType.ECVOTE => ("ECVOTE", "Ethics Committee Vote"),
                PublicationType.TRIALPROTOCOL => ("trial-protocol", "Trial Protocol"),
                PublicationType.ABSTRACT => ("ABSTRACT", "Abstract"),
                PublicationType.RESULT => ("RESULT", "Result"),
                PublicationType.LITERATURE => ("LITERATURE", "Literature"),
                PublicationType.BASICREPORTING => ("BASIC_REPORTING", "Basic reporting"),
                PublicationType.ADDITONAL_VOTE => ("ADDITIONAL_VOTE", "Additional vote"),
                _ => ("OTHER", "Other")
            };

            study.RelatedArtifact.Add(new RelatedArtifact
            {
                Type = RelatedArtifact.RelatedArtifactType.Citation,
                Classifier =
                [
                    new CodeableConcept(
                        "https://www.imise.uni-leipzig.de/fhir/CodeSystem/PublicationType",
                        code, display)
                ],
                Display = pub.Description,
                Url = pub.Link
            });
        }
    }
}