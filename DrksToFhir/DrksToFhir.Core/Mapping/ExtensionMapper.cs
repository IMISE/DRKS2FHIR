using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ExtensionMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.Extension ??= [];

        MapEthicsCommitteeExtension(drks, study);
        MapTrialResultExtension(drks, study);
        MapStudyCharacteristicDescriptionExtensions(drks, study);
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        MapTrialResultExtensionReverse(study, drks);
        MapStudyCharacteristicDescriptionExtensionsReverse(study, drks);
    }

    private static void MapEthicsCommitteeExtension(DrksStudy drks, ResearchStudy study)
    {
        if (drks.EthicsCommittee != null)
        {
            var ec = drks.EthicsCommittee;
            var ecExt = new Extension
            {
                Url = "https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-ethics-committee"
            };

            if (ec.ApplicationDate != null)
            {
                ecExt.Extension.Add(new Extension("applicationDate", new Date(ec.ApplicationDate)));
            }                

            if (ec.Number != null)
            {
                ecExt.Extension.Add(new Extension("number", new FhirString(ec.Number)));
            }                

            if (ec.EthicVotes != null)
            {
                foreach (var vote in ec.EthicVotes)
                {
                    if (vote.Vote != null)
                    {
                        ecExt.Extension.Add(new Extension("vote", new Coding(
                            "https://www.imise.uni-leipzig.de/fhir/CodeSystem/EthicVote",
                            vote.Vote.ToString()!.ToLowerInvariant().Replace("_", "-")
                        )));
                    }                        

                    if (vote.DateOfVote != null)
                    {
                        ecExt.Extension.Add(new Extension("dateOfVote", new Date(vote.DateOfVote)));
                    }                        

                    if (vote.Type != null)
                    {
                        ecExt.Extension.Add(new Extension("type", new Coding(
                            "https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType",
                            vote.Type.ToString()!.ToLowerInvariant().Replace("_", "-")
                        )));
                    }
                }
            }

            study.Extension.Add(ecExt);
        }
    }

    private static void MapTrialResultExtension(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialResults != null)
        {
            var tr = drks.TrialResults;
            var trExt = new Extension
            {
                Url = "https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-trial-result"
            };

            if (tr.IpdSharingPlan != null)
            {
                trExt.Extension.Add(new Extension("ipd-sharing-plan", new FhirBoolean(tr.IpdSharingPlan)));
            }

            if (tr.PlannedPublication != null)
            {
                trExt.Extension.Add(new Extension("planned-publication", new Date(tr.PlannedPublication)));
            }

            if (tr.FirstPublication != null)
            {
                trExt.Extension.Add(new Extension("first-publication", new Date(tr.FirstPublication)));
            }

            if (tr.FirstResultPublication != null)
            {
                trExt.Extension.Add(new Extension("first-result-publication", new Date(tr.FirstResultPublication)));
            }

            study.Extension.Add(trExt);
        }
    }

    private static void MapTrialResultExtensionReverse(ResearchStudy study, DrksStudy drks)
    {
        var trExt = study.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-trial-result");
        if (trExt != null)
        {
            drks.TrialResults ??= new TrialResults();
            drks.TrialResults.IpdSharingPlan = (trExt.Extension.FirstOrDefault(e => e.Url == "ipd-sharing-plan")?.Value as FhirBoolean)?.Value;
            drks.TrialResults.PlannedPublication = (trExt.Extension.FirstOrDefault(e => e.Url == "planned-publication")?.Value as Date)?.Value;
            drks.TrialResults.FirstPublication = (trExt.Extension.FirstOrDefault(e => e.Url == "first-publication")?.Value as Date)?.Value;
            drks.TrialResults.FirstResultPublication = (trExt.Extension.FirstOrDefault(e => e.Url == "first-result-publication")?.Value as Date)?.Value;
        }
    }

    private static void MapStudyCharacteristicDescriptionExtensions(DrksStudy drks, ResearchStudy study)
    {
        var enDesc = drks.StudyCharacteristic?.CharacteristicDescriptions?.FirstOrDefault(d => d.IdLocale?.Locale == Locale.en);
        var deDesc = drks.StudyCharacteristic?.CharacteristicDescriptions?.FirstOrDefault(d => d.IdLocale?.Locale == Locale.de);

        if (enDesc?.TypeOfSequenceGeneration != null || deDesc?.TypeOfSequenceGeneration != null)
        {
            study.Extension.Add(new Extension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-sequence-generation-description", FhirHelper.BuildTranslatedString(enDesc?.TypeOfSequenceGeneration, deDesc?.TypeOfSequenceGeneration)));
        }

        if (enDesc?.TypeOfHiddenAllocation != null || deDesc?.TypeOfHiddenAllocation != null)
        {
            study.Extension.Add(new Extension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-hidden-allocation-description", FhirHelper.BuildTranslatedString(enDesc?.TypeOfHiddenAllocation, deDesc?.TypeOfHiddenAllocation)));
        }
    }

    private static void MapStudyCharacteristicDescriptionExtensionsReverse(ResearchStudy study, DrksStudy drks)
    {
        var seqExt = study.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-sequence-generation-description");
        var hiddenExt = study.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-hidden-allocation-description");

        if (seqExt == null && hiddenExt == null)
        {
            return;
        }

        drks.StudyCharacteristic ??= new StudyCharacteristic();
        drks.StudyCharacteristic.CharacteristicDescriptions ??= [];

        var enSeq = (seqExt?.Value as PrimitiveType)?.ToString();
        var deSeq = TitleDescriptionMapper.GetTranslation(seqExt?.Value as PrimitiveType);
        var enHidden = (hiddenExt?.Value as PrimitiveType)?.ToString();
        var deHidden = TitleDescriptionMapper.GetTranslation(hiddenExt?.Value as PrimitiveType);

        if (enSeq != null || enHidden != null)
        {
            var enDesc = GetOrAddDescription(drks.StudyCharacteristic, Locale.en);
            enDesc.TypeOfSequenceGeneration = enSeq;
            enDesc.TypeOfHiddenAllocation = enHidden;
        }

        if (deSeq != null || deHidden != null)
        {
            var deDesc = GetOrAddDescription(drks.StudyCharacteristic, Locale.de);
            deDesc.TypeOfSequenceGeneration = deSeq;
            deDesc.TypeOfHiddenAllocation = deHidden;
        }
    }

    internal static CharacteristicDescription GetOrAddDescription(StudyCharacteristic sc, Locale locale)
    {
        var existing = sc.CharacteristicDescriptions!.FirstOrDefault(d => d.IdLocale?.Locale == locale);
        if (existing != null)
        {
            return existing;
        }

        var desc = new CharacteristicDescription { IdLocale = new IdLocale { Locale = locale } };
        sc.CharacteristicDescriptions!.Add(desc);
        return desc;
    }
}