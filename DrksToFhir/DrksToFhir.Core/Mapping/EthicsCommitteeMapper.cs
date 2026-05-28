using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class EthicsCommitteeMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.EthicsCommittee?.EthicVotes is null) return;

        study.Contained ??= [];
        study.AssociatedParty ??= [];

        foreach (var vote in drks.EthicsCommittee.EthicVotes)
        {
            var orgId = $"{drks.DrksId}-Ethics-Committee-Contact";
            study.Contained.Add(FhirHelper.BuildOrganization(orgId, vote.Contact));

            study.AssociatedParty.Add(new ResearchStudy.AssociatedPartyComponent
            {
                Role = new CodeableConcept(
                    "http://hl7.org/fhir/research-study-party-role",
                    "irb", "Institutional Review Board"),
                Party = new ResourceReference($"#{orgId}", "Organization")
            });
        }
    }
}