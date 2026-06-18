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

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        var contacts = new List<TrialContact>();
        var materialSupports = new List<MaterialSupport>();

        foreach (var ap in study.AssociatedParty)
        {
            var roleCode = ap.Role?.Coding.FirstOrDefault()?.Code;
            var roleSystem = ap.Role?.Coding.FirstOrDefault()?.System;
            var orgId = ap.Party?.Reference?.TrimStart('#');
            var org = study.Contained.OfType<Organization>().FirstOrDefault(o => o.Id == orgId);

            if (org is null)
            {
                continue;
            }

            var contact = BuildDrksContact(org);

            if (roleSystem == "https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType" 
                && roleCode == "commercial"
                || roleCode == "private-sponsor" 
                || roleCode == "institutional")
            {
                MaterialSupportType? msType = roleCode switch
                {
                    "commercial" => MaterialSupportType.COMMERCIAL,
                    "private-sponsor" => MaterialSupportType.PRIVATE_SPONSORSHIP,
                    "institutional" => MaterialSupportType.INSTITUTIONAL,
                    _ => null
                };
                if (msType.HasValue)
                {
                    materialSupports.Add(new MaterialSupport { Type = msType, Contact = contact });
                }
            }
            else if (roleCode == "funding-source")
            {
                materialSupports.Add(new MaterialSupport
                {
                    Type = MaterialSupportType.PUBLIC_FUNDING,
                    Contact = contact
                });
            }
            else if (roleCode == "irb")
            {
                continue;
            }
            else
            {
                TrialContactType? contactType = roleCode switch
                {
                    "lead-sponsor" => TrialContactType.PRIMARY_SPONSOR,
                    "sponsor" => TrialContactType.SECONDARY_SPONSOR,
                    "primary-investigator" => TrialContactType.PRINCIPAL_COORDINATING_INVESTIGATOR,
                    "general-contact" => TrialContactType.PUBLIC_QUERIES,
                    "scientific-contact" => TrialContactType.SCIENTIFIC_QUERIES,
                    "collaborator" => TrialContactType.COLLABORATOR_OTHER_ADDRESS,
                    _ => null
                };

                if (contactType.HasValue)
                {
                    OtherContactType? otherContactType = null;
                    if(contactType == TrialContactType.COLLABORATOR_OTHER_ADDRESS && ap.Role?.Text is string text && Enum.TryParse<OtherContactType>(text, out var parsed))
                    {
                        otherContactType = parsed;
                    }


                    contacts.Add(new TrialContact
                    {
                        IdContactIdType = new IdContactIdType { Type = contactType.Value },
                        Contact = contact,
                        OtherType = otherContactType
                    });
                }
            }            
        }

        drks.TrialContacts = contacts.Count > 0 ? contacts : null;
        drks.MaterialSupports = materialSupports.Count > 0 ? materialSupports : null;
        drks.Sponsored = study.AssociatedParty.Any(ap => ap.Role?.Coding.Any(c => c.Code == "lead-sponsor" || c.Code == "sponsor") == true);
    }

    internal static DrksContact BuildDrksContact(Organization org)
    {
        var contactInfo = org.Contact.FirstOrDefault();
        var humanName = contactInfo?.Name.FirstOrDefault();
        return new DrksContact
        {
            Affiliation = org.Name,
            FirstName = humanName?.Given.FirstOrDefault(),
            LastName = humanName?.Family,
            Title = humanName?.Prefix.FirstOrDefault(),
            Email = contactInfo?.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Email)?.Value,
            Phone = contactInfo?.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Phone)?.Value,
            Fax = contactInfo?.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Fax)?.Value,
            Url = contactInfo?.Telecom.FirstOrDefault(t => t.System == ContactPoint.ContactPointSystem.Url)?.Value,
            StreetAndNo = contactInfo?.Address?.Line.FirstOrDefault(),
            City = contactInfo?.Address?.City,
            Zip = contactInfo?.Address?.PostalCode,
            Country = contactInfo?.Address?.Country
        };
    }

    private static void MapTrialContacts(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialContacts != null)
        {
            int index = 0;
            foreach (var tc in drks.TrialContacts)
            {
                var orgId = $"{drks.DrksId}-AssociatedParty-{index++}";
                study.Contained.Add(FhirHelper.BuildOrganization(orgId, tc.Contact));

                var (roleSystem, roleCode, roleDisplay) = tc.IdContactIdType?.Type switch
                {
                    TrialContactType.PRIMARY_SPONSOR => ("http://hl7.org/fhir/research-study-party-role", "lead-sponsor", "Lead sponsor"),
                    TrialContactType.SECONDARY_SPONSOR => ("http://hl7.org/fhir/research-study-party-role", "sponsor", "Sponsor"),
                    TrialContactType.PRINCIPAL_COORDINATING_INVESTIGATOR => ("http://hl7.org/fhir/research-study-party-role", "primary-investigator", "Principal investigator"),
                    TrialContactType.PUBLIC_QUERIES => ("http://hl7.org/fhir/research-study-party-role", "general-contact", "General contact"),
                    TrialContactType.SCIENTIFIC_QUERIES => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType", "scientific-contact", "Scientific contact"),
                    _ => ("http://hl7.org/fhir/research-study-party-role", "collaborator", "collaborator")
                };

                var role = new CodeableConcept(roleSystem, roleCode, roleDisplay);
                if (tc.OtherType != null)
                {
                    role.Text = tc.OtherType.ToString();
                }                   

                study.AssociatedParty.Add(new ResearchStudy.AssociatedPartyComponent
                {
                    Role = role,
                    Party = new ResourceReference($"#{orgId}", "Organization")
                });
            }
        }
    }

    private static void MapMaterialSupports(DrksStudy drks, ResearchStudy study)
    {
        if (drks.MaterialSupports != null)
        {
            int index = drks.TrialContacts?.Count ?? 0;
            foreach (var ms in drks.MaterialSupports)
            {
                var orgId = $"{drks.DrksId}-AssociatedParty-{index++}";
                study.Contained.Add(FhirHelper.BuildOrganization(orgId, ms.Contact));

                var (roleSystem, roleCode, roleDisplay) = ms.Type switch
                {
                    MaterialSupportType.COMMERCIAL => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType", "commercial", "Commercial"),
                    MaterialSupportType.PUBLIC_FUNDING => ("http://hl7.org/fhir/research-study-party-role", "funding-source", "Funding source"),
                    MaterialSupportType.PRIVATE_SPONSORSHIP => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType", "private-sponsor", "Private sponsorship"),
                    MaterialSupportType.INSTITUTIONAL => ("https://www.imise.uni-leipzig.de/fhir/CodeSystem/ContactType", "institutional", "Institutional"),
                    _ => ("http://hl7.org/fhir/research-study-party-role", "funding-source", "Funding source")
                };

                study.AssociatedParty.Add(new ResearchStudy.AssociatedPartyComponent
                {
                    Role = new CodeableConcept(roleSystem, roleCode, roleDisplay),
                    Party = new ResourceReference($"#{orgId}", "Organization")
                });
            }
        }
    }
}