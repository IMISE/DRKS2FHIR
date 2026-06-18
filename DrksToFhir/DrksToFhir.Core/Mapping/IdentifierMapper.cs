using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class IdentifierMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.Identifier = [];

        if (drks.DrksId != null)
        {
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "drksId" },
                System = "https://drks.de",
                Value = drks.DrksId
            });
        }

        if(drks.SecondaryIds?.OtherPrimaryRegisterId != null || drks.SecondaryIds?.OtherPrimaryRegisterName != null)
        {
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = $"otherPrimaryRegisterId:{drks.SecondaryIds?.OtherPrimaryRegisterName}" },
                Value = drks.SecondaryIds?.OtherPrimaryRegisterId
            });
        }

        if (drks.SecondaryIds?.UniversalTrialNumber != null)
        {
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "universalTrialNumber" },
                System = "https://trialsearch.who.int/utn.aspx",
                Value = drks.SecondaryIds.UniversalTrialNumber
            });
        }            

        if (drks.SecondaryIds?.EudraCtNumber != null)
        {
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "eudraCtNumber" },
                System = "https://eudract.ema.europa.eu",
                Value = drks.SecondaryIds.EudraCtNumber
            });
        }            

        if (drks.SecondaryIds?.EudamedNumber != null)
        {
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "eudamedNumber" },
                System = "https://eudamed.ec.europa.eu",
                Value = drks.SecondaryIds.EudamedNumber
            });
        }
        
        foreach(var id in drks.OtherSecondaryIds ?? Enumerable.Empty<OtherSecondaryId>())
        {
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = $"otherSecondaryId:{id.Description}" },
                Value = id.Value
            });
        }

        study.Name = drks.DrksId != null ? $"{drks.DrksId} FHIR" : null;
        study.Id = drks.DrksId != null ? $"{drks.DrksId}-FHIR" : null;
        study.Url = drks.Url;
        study.Date = drks.LastUpdate;
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        drks.DrksId = GetIdentifierValue(study, "https://drks.de");
        drks.Url = study.Url;
        drks.LastUpdate = study.Date;

        var utn = GetIdentifierValue(study, "https://trialsearch.who.int/utn.aspx");
        var eudract = GetIdentifierValue(study, "https://eudract.ema.europa.eu");
        var eudamed = GetIdentifierValue(study, "https://eudamed.ec.europa.eu");

        var otherPrimary = study.Identifier.FirstOrDefault(id => id.Type?.Text?.StartsWith("otherPrimaryRegisterId:") == true);
        string? otherPrimaryName = otherPrimary?.Type?.Text is string t1
            ? t1["otherPrimaryRegisterId:".Length..]
            : null;
        if (otherPrimaryName == "")
        {
            otherPrimaryName = null;
        }

        var otherSecondaryIds = study.Identifier.Where(id => id.Type?.Text?.StartsWith("otherSecondaryId:") == true && id.Value != null)
            .Select(id =>
            {
                var desc = id.Type!.Text!["otherSecondaryId:".Length..];
                return new OtherSecondaryId
                {
                    Value = id.Value,
                    Description = desc == "" ? null : desc
                };
            })
            .ToList();

        drks.SecondaryIds = new SecondaryIds
        {
            UniversalTrialNumber = utn,
            EudraCtNumber = eudract,
            EudamedNumber = eudamed,
            OtherPrimaryRegisterId = otherPrimary?.Value,
            OtherPrimaryRegisterName = otherPrimaryName,
            NoOtherIdentificationNumbersAvailable = utn == null && eudract == null && eudamed == null && otherPrimary == null
        };

        drks.OtherSecondaryIds = otherSecondaryIds.Count > 0 ? otherSecondaryIds : null;
    }

    private static string? GetIdentifierValue(ResearchStudy study, string system)
    {
        return study.Identifier.FirstOrDefault(id => id.System == system)?.Value;
    }
        
}