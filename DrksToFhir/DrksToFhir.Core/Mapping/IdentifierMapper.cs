using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class IdentifierMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.Identifier = [];

        if (drks.DrksId is not null)
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "drksId" },
                System = "https://drks.de",
                Value = drks.DrksId
            });

        if (drks.SecondaryIds?.UniversalTrialNumber is not null)
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "universalTrialNumber" },
                System = "https://trialsearch.who.int/utn.aspx",
                Value = drks.SecondaryIds.UniversalTrialNumber
            });

        if (drks.SecondaryIds?.EudraCtNumber is not null)
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "eudraCtNumber" },
                System = "https://eudract.ema.europa.eu",
                Value = drks.SecondaryIds.EudraCtNumber
            });

        if (drks.SecondaryIds?.EudamedNumber is not null)
            study.Identifier.Add(new Identifier
            {
                Use = Identifier.IdentifierUse.Official,
                Type = new CodeableConcept { Text = "eudamedNumber" },
                System = "https://eudamed.ec.europa.eu",
                Value = drks.SecondaryIds.EudamedNumber
            });

        study.Name = drks.DrksId is not null ? $"{drks.DrksId} FHIR" : null;
        study.Id = drks.DrksId is not null ? $"{drks.DrksId}-FHIR" : null;
        study.Url = drks.Url;
        study.Date = drks.LastUpdate;
    }
}