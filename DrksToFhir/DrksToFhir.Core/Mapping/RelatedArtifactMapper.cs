using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class RelatedArtifactMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialResults?.Publications != null)
        {
            study.RelatedArtifact = [];

            foreach (var pub in drks.TrialResults.Publications)
            {
                var (code, display) = pub.Type switch
                {
                    PublicationType.ECVOTE => ("ecvote", "Ethics Committee Vote"),
                    PublicationType.TRIALPROTOCOL => ("trial-protocol", "Trial Protocol"),
                    PublicationType.ABSTRACT => ("abstract", "Abstract"),
                    PublicationType.RESULT => ("result", "Result"),
                    PublicationType.LITERATURE => ("literature", "Literature"),
                    PublicationType.BASICREPORTING => ("basic-reporting", "Basic reporting"),
                    PublicationType.ADDITONAL_VOTE => ("additional-vote", "Additional vote"),
                    _ => ("other", "Other")
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

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        if (study.RelatedArtifact.Count > 0)
        {
            var publications = study.RelatedArtifact.Select(ra =>
            {
                var typeCode = ra.Classifier.FirstOrDefault()?.Coding.FirstOrDefault()?.Code;
                PublicationType? pubType = typeCode?.ToUpperInvariant().Replace('-', '_') switch
                {
                    "ECVOTE" => PublicationType.ECVOTE,
                    "TRIAL_PROTOCOL" => PublicationType.TRIALPROTOCOL,
                    "ABSTRACT" => PublicationType.ABSTRACT,
                    "RESULT" => PublicationType.RESULT,
                    "LITERATURE" => PublicationType.LITERATURE,
                    "BASIC_REPORTING" => PublicationType.BASICREPORTING,
                    "ADDITIONAL_VOTE" => PublicationType.ADDITONAL_VOTE,
                    _ => PublicationType.OTHER
                };

                return new Publication
                {
                    Description = ra.Display,
                    Link = ra.Url,
                    Type = pubType
                };
            }).ToList();

            if (publications.Count > 0)
            {
                drks.TrialResults ??= new TrialResults();
                drks.TrialResults.Publications = publications;
            }
        }
    }
}