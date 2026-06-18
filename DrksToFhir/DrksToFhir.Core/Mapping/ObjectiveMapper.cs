using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ObjectiveMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.StudyCharacteristic?.CharacteristicDescriptions != null)
        {
            study.Objective = [];

            var enDesc = drks.StudyCharacteristic.CharacteristicDescriptions.FirstOrDefault(d => d.IdLocale?.Locale == Locale.en);
            var deDesc = drks.StudyCharacteristic.CharacteristicDescriptions.FirstOrDefault(d => d.IdLocale?.Locale == Locale.de);

            if (enDesc?.PrimaryOutcome != null || deDesc?.PrimaryOutcome != null)
            {
                study.Objective.Add(new ResearchStudy.ObjectiveComponent
                {
                    Name = "primary",
                    DescriptionElement = FhirHelper.BuildTranslatedMarkdown(enDesc?.PrimaryOutcome, deDesc?.PrimaryOutcome)
                });
            }

            if (enDesc?.SecondaryOutcome != null || deDesc?.SecondaryOutcome != null)
            {
                study.Objective.Add(new ResearchStudy.ObjectiveComponent
                {
                    Name = "secondary",
                    DescriptionElement = FhirHelper.BuildTranslatedMarkdown(enDesc?.SecondaryOutcome, deDesc?.SecondaryOutcome)
                });
            }
        }
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        if (study.Objective.Count > 0)
        {

        }

        drks.StudyCharacteristic ??= new StudyCharacteristic();

        var enDesc = new CharacteristicDescription { IdLocale = new IdLocale { Locale = Locale.en } };
        var deDesc = new CharacteristicDescription { IdLocale = new IdLocale { Locale = Locale.de } };

        foreach (var obj in study.Objective)
        {
            var isPrimary = obj.Name == "primary";
            var enText = obj.Description;
            var deText = TitleDescriptionMapper.GetTranslation(obj.DescriptionElement);

            if (isPrimary)
            {
                enDesc.PrimaryOutcome = enText;
                deDesc.PrimaryOutcome = deText;
            }
            else
            {
                enDesc.SecondaryOutcome = enText;
                deDesc.SecondaryOutcome = deText;
            }
        }

        var seqExt = study.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-sequence-generation-description");
        if (seqExt?.Value is PrimitiveType seqValue)
        {
            enDesc.TypeOfSequenceGeneration = seqValue.ToString();
            deDesc.TypeOfSequenceGeneration = TitleDescriptionMapper.GetTranslation(seqValue);
        }

        var hiddenExt = study.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-hidden-allocation-description");
        if (hiddenExt?.Value is PrimitiveType hiddenValue)
        {
            enDesc.TypeOfHiddenAllocation = hiddenValue.ToString();
            deDesc.TypeOfHiddenAllocation = TitleDescriptionMapper.GetTranslation(hiddenValue);
        }

        var descriptions = new List<CharacteristicDescription>();
        if (enDesc.PrimaryOutcome != null || enDesc.SecondaryOutcome != null || enDesc.TypeOfSequenceGeneration != null || enDesc.TypeOfHiddenAllocation != null)
        {
            descriptions.Add(enDesc);
        }            
        if (deDesc.PrimaryOutcome != null || deDesc.SecondaryOutcome != null || deDesc.TypeOfSequenceGeneration != null || deDesc.TypeOfHiddenAllocation != null)
        {
            descriptions.Add(deDesc);
        }

        drks.StudyCharacteristic.CharacteristicDescriptions = descriptions.Count > 0 ? descriptions : null;
    }
}