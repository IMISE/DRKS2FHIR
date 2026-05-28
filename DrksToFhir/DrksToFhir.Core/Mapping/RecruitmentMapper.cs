using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class RecruitmentMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.Recruitment is null) return;

        var rec = drks.Recruitment;

        MapRegion(rec, study);
        MapSites(drks, rec, study);
        MapEligibility(drks, rec, study);
    }

    private static void MapRegion(Recruitment rec, ResearchStudy study)
    {
        if (rec.Countries is null) return;

        study.Region = rec.Countries
            .Where(c => c.IdCountry?.Code is not null)
            .Select(c => new CodeableConcept(
                "urn:iso:std:iso:3166", c.IdCountry!.Code,
                c.IdCountry.Code == "DE" ? "Germany" : c.IdCountry.Code))
            .ToList();
    }

    private static void MapSites(DrksStudy drks, Recruitment rec, ResearchStudy study)
    {
        if (rec.Institutes is null) return;

        study.Contained ??= [];
        study.Site = [];

        for (int i = 0; i < rec.Institutes.Count; i++)
        {
            var inst = rec.Institutes[i];
            var locId = $"{drks.DrksId}-Recruitment-Institute-{i}";

            var (roleCode, roleDisplay, typeSystem) = inst.Type switch
            {
                InstituteType.UNI_MEDICAL_CENTER => (
                    "UMC", "Uni Medical Center",
                    "https://www.imise.uni-leipzig.de/fhir/CodeSystem/InstituteType"),
                InstituteType.MEDICAL_CENTER => (
                    "HOSP", "Hospital",
                    "http://terminology.hl7.org/CodeSystem/v3-RoleCode"),
                _ => (
                    "OF", "Outpatient facility",
                    "http://terminology.hl7.org/CodeSystem/v3-RoleCode")
            };

            var location = new Location
            {
                Id = locId,
                Name = inst.Name,
                Address = new Address { City = inst.City },
                Type = [new CodeableConcept(typeSystem, roleCode, roleDisplay)]
            };

            study.Contained.Add(location);
            study.Site.Add(new ResourceReference($"#{locId}", "Location"));
        }
    }

    private static void MapEligibility(DrksStudy drks, Recruitment rec, ResearchStudy study)
    {
        var groupId = $"{drks.DrksId}-Recruitment-Eligibility";
        var group = new Group
        {
            Id = groupId,
            Type = Group.GroupType.Person,
            Membership = Group.GroupMembershipBasis.Definitional,
            Characteristic = []
        };

        if (rec.Gender is not null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept("http://loinc.org", "76691-5", "Gender identity"),
                Value = new CodeableConcept { Text = rec.Gender.ToString() },
                Exclude = false
            });
        }

        var ageChar = FhirHelper.BuildAgeCharacteristic(rec.MinimumAge, rec.MaximumAge);
        if (ageChar is not null)
            group.Characteristic.Add(ageChar);

        var enRecDesc = rec.RecruitmentDescriptions?
            .FirstOrDefault(d => d.IdLocale.Locale == Locale.en);
        var deRecDesc = rec.RecruitmentDescriptions?
            .FirstOrDefault(d => d.IdLocale.Locale == Locale.de);

        if (enRecDesc?.ExclusionCriteria is not null || deRecDesc?.ExclusionCriteria is not null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept { Text = "FREETEXT" },
                Value = new CodeableConcept
                {
                    TextElement = FhirHelper.BuildTranslatedString(
                        enRecDesc?.ExclusionCriteria, deRecDesc?.ExclusionCriteria)
                },
                Exclude = true
            });
        }

        if (enRecDesc?.InclusionCriteria is not null || deRecDesc?.InclusionCriteria is not null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept { Text = "FREETEXT" },
                Value = new CodeableConcept
                {
                    TextElement = FhirHelper.BuildTranslatedString(
                        enRecDesc?.InclusionCriteria, deRecDesc?.InclusionCriteria)
                },
                Exclude = false
            });
        }

        if (rec.HealthyVolunteers is not null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept(
                    "https://www.imise.uni-leipzig.de/fhir/CodeSystem/EligibilityCriteria",
                    "healthyVolunteers", "Healthy volunteers"),
                Value = new FhirBoolean(rec.HealthyVolunteers),
                Exclude = false
            });
        }

        study.Contained ??= [];
        study.Contained.Add(group);

        study.Recruitment = new ResearchStudy.RecruitmentComponent
        {
            TargetNumber = rec.PlannedParticipants,
            ActualNumber = rec.TotalParticipants,
            Eligibility = new ResourceReference($"#{groupId}", "Group")
        };
    }
}