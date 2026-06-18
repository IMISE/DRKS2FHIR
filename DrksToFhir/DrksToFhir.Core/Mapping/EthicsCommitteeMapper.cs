using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class EthicsCommitteeMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.EthicsCommittee?.EthicVotes != null)
        {
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

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        var ecExt = study.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-ethics-committee");
        if (ecExt != null)
        {
            drks.EthicsCommittee = new EthicsCommittee
            {
                ApplicationDate = (ecExt.Extension.FirstOrDefault(e => e.Url == "applicationDate")?.Value as Date)?.Value,
                Number = (ecExt.Extension.FirstOrDefault(e => e.Url == "number")?.Value as FhirString)?.Value
            };

            var voteCoding = ecExt.Extension.FirstOrDefault(e => e.Url == "vote")?.Value as Coding;
            var dateOfVote = (ecExt.Extension.FirstOrDefault(e => e.Url == "dateOfVote")?.Value as Date)?.Value;
            var typeCoding = ecExt.Extension.FirstOrDefault(e => e.Url == "type")?.Value as Coding;

            var ecParty = study.AssociatedParty.FirstOrDefault(ap => ap.Role?.Coding.Any(c => c.Code == "irb") == true);
            var ecOrgId = ecParty?.Party?.Reference?.TrimStart('#');
            var ecOrg = study.Contained.OfType<Organization>().FirstOrDefault(o => o.Id == ecOrgId);

            EthicVoteResult? voteResult = null;
            if (voteCoding?.Code is string voteCode && Enum.TryParse<EthicVoteResult>(voteCode.ToUpperInvariant().Replace('-', '_'), out var vr))
            {
                voteResult = vr;
            }                

            TrialContactType? contactType = null;
            if (typeCoding?.Code is string typeCode && Enum.TryParse<TrialContactType>(typeCode.ToUpperInvariant().Replace('-', '_'), out var ct))
            {
                contactType = ct;
            }                

            drks.EthicsCommittee.EthicVotes = [ new EthicVote
            {
                Vote = voteResult,
                DateOfVote = dateOfVote,
                Type = contactType,
                Contact = ecOrg is not null ? ContactMapper.BuildDrksContact(ecOrg) : null
            }];
        }
    }
}