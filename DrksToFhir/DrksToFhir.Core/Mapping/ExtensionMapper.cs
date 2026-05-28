using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ExtensionMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.Extension ??= [];

        MapTrialStatus(drks, study);
        MapEthicsCommitteeExtension(drks, study);
        MapTrialResultExtension(drks, study);
    }

    private static void MapTrialStatus(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialStatus is null) return;

        study.Extension.Add(new Extension(
            "https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-trial-status",
            new Code(drks.TrialStatus.ToString())));
    }

    private static void MapEthicsCommitteeExtension(DrksStudy drks, ResearchStudy study)
    {
        if (drks.EthicsCommittee is null) return;

        var ec = drks.EthicsCommittee;
        var ecExt = new Extension
        {
            Url = "https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-ethics-committee"
        };

        if (ec.ApplicationDate is not null)
            ecExt.Extension.Add(new Extension("applicationDate",
                new Date(ec.ApplicationDate)));

        if (ec.Number is not null)
            ecExt.Extension.Add(new Extension("number",
                new FhirString(ec.Number)));

        if (ec.EthicVotes is not null)
        {
            foreach (var vote in ec.EthicVotes)
            {
                if (vote.Vote is not null)
                    ecExt.Extension.Add(new Extension("vote",
                        new Coding(
                            "https://www.imise.uni-leipzig.de/fhir/CodeSystem/EthicVote",
                            vote.Vote.ToString())));

                if (vote.DateOfVote is not null)
                    ecExt.Extension.Add(new Extension("dateOfVote",
                        new Date(vote.DateOfVote)));

                if (vote.Type is not null)
                    ecExt.Extension.Add(new Extension("type",
                        new Code(vote.Type.ToString())));
            }
        }

        study.Extension.Add(ecExt);
    }

    private static void MapTrialResultExtension(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialResults is null) return;

        var tr = drks.TrialResults;
        var trExt = new Extension
        {
            Url = "https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-trial-result"
        };

        if (tr.IpdSharingPlan is not null)
            trExt.Extension.Add(new Extension("ipd-sharing-plan",
                new FhirBoolean(tr.IpdSharingPlan)));

        if (tr.PlannedPublication is not null)
            trExt.Extension.Add(new Extension("planned-publication",
                new FhirString(tr.PlannedPublication)));

        if (tr.FirstPublication is not null)
            trExt.Extension.Add(new Extension("first-publication",
                new Date(tr.FirstPublication)));

        if (tr.FirstResultPublication is not null)
            trExt.Extension.Add(new Extension("first-result-publication",
                new Date(tr.FirstResultPublication)));

        study.Extension.Add(trExt);
    }
}