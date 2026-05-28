using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class ContactMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.Contained ??= [];
        study.AssociatedParty = [];

        MapTrialContacts(drks, study);
        MapMaterialSupports(drks, study);
    }

    private static void MapTrialContacts(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialContacts is null) return;

        int index = 0;
        foreach (var tc in drks.TrialContacts)
        {
            var orgId = $"{drks.DrksId}-AssociatedParty-{index++}";
            study.Contained.Add(FhirHelper.BuildOrganization(orgId, tc.Contact));

            var (roleSystem, roleCode, roleDisplay) = tc.IdContactIdType?.Type switch
            {
                TrialContactType.PRIMARY_SPONSOR => (
                    "http://hl7.org/fhir/research-study-party-role",
                    "lead-sponsor", "Lead sponsor"),
                TrialContactType.SECONDARY_SPONSOR => (
                    "http://hl7.org/fhir/research-study-party-role",
                    "sponsor", "Sponsor"),
                TrialContactType.PRINCIPAL_COORDINATING_INVESTIGATOR => (
                    "http://hl7.org/fhir/research-study-party-role",
                    "primary-investigator", "Principal investigator"),
                TrialContactType.PUBLIC_QUERIES => (
                    "http://hl7.org/fhir/research-study-party-role",
                    "general-contact", "General contact"),
                TrialContactType.SCIENTIFIC_QUERIES => (
                    "https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType",
                    "SCIENTIFIC_CONTACT", "Scientific contact"),
                _ => (
                    "http://hl7.org/fhir/research-study-party-role",
                    "collaborator", "collaborator")
            };

            var role = new CodeableConcept(roleSystem, roleCode, roleDisplay);
            if (tc.OtherType is not null)
                role.Text = tc.OtherType.ToString();

            study.AssociatedParty.Add(new ResearchStudy.AssociatedPartyComponent
            {
                Role = role,
                Party = new ResourceReference($"#{orgId}", "Organization")
            });
        }
    }

    private static void MapMaterialSupports(DrksStudy drks, ResearchStudy study)
    {
        if (drks.MaterialSupports is null) return;

        int index = drks.TrialContacts?.Count ?? 0;
        foreach (var ms in drks.MaterialSupports)
        {
            var orgId = $"{drks.DrksId}-AssociatedParty-{index++}";
            study.Contained.Add(FhirHelper.BuildOrganization(orgId, ms.Contact));

            var (roleSystem, roleCode, roleDisplay) = ms.Type switch
            {
                MaterialSupportType.COMMERCIAL => (
                    "https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType",
                    "COMMERCIAL", "Commercial"),
                MaterialSupportType.PUBLIC_FUNDING => (
                    "http://hl7.org/fhir/research-study-party-role",
                    "funding-source", "Funding source"),
                MaterialSupportType.PRIVATE_SPONSORSHIP => (
                    "https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType",
                    "PRIVATE_SPONSOR", "Private sponsorship"),
                MaterialSupportType.INSTITUTIONAL => (
                    "https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType",
                    "INSTITUTIONAL", "Institutional"),
                _ => (
                    "http://hl7.org/fhir/research-study-party-role",
                    "funding-source", "Funding source")
            };

            study.AssociatedParty.Add(new ResearchStudy.AssociatedPartyComponent
            {
                Role = new CodeableConcept(roleSystem, roleCode, roleDisplay),
                Party = new ResourceReference($"#{orgId}", "Organization")
            });
        }
    }
}