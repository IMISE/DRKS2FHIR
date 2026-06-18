using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class RecruitmentMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        if (drks.Recruitment != null)
        {
            var rec = drks.Recruitment;

            MapRegion(rec, study);
            MapSites(drks, rec, study);
            MapEligibility(drks, rec, study);
        }
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        drks.Recruitment ??= new Recruitment();
        var r = drks.Recruitment;

        r.PlannedParticipants = study.Recruitment?.TargetNumber;
        r.TotalParticipants = study.Recruitment?.ActualNumber;

        r.CentricType = (study.Site?.Count ?? 0) > 1
            ? CentricType.MULTICENTRIC
            : CentricType.MONOCENTRIC;

        if (study.Region?.Count > 0)
        {
            r.Countries = study.Region.Select(region => new RecruitmentCountry
            {
                IdCountry = new IdCountry 
                { 
                    Code = region.Coding.FirstOrDefault()?.Code 
                }
            }).Where(c => c.IdCountry?.Code != null).ToList();
        }

        var locations = study.Contained.OfType<Location>().ToList();
        if (locations.Count > 0)
        {
            r.Institutes = locations.Select(loc =>
            {
                var typeCode = loc.Type.FirstOrDefault()?.Coding.FirstOrDefault()?.Code;
                InstituteType? instType = typeCode switch
                {
                    "umc" or "UMC" => InstituteType.UNI_MEDICAL_CENTER,
                    "HOSP" => InstituteType.MEDICAL_CENTER,
                    "OF" => InstituteType.PRACTICE,
                    _ => InstituteType.OTHER
                };
                return new Institute
                {
                    Name = loc.Name,
                    City = loc.Address?.City,
                    Type = instType
                };
            }).ToList();
        }

        var eligibilityRef = study.Recruitment?.Eligibility?.Reference?.TrimStart('#');
        var group = study.Contained.OfType<Group>().FirstOrDefault(g => g.Id == eligibilityRef);
        if (group != null)
        {
            MapEligibilityReverse(group, r);
        }            
    }

    private static void MapRegion(Recruitment rec, ResearchStudy study)
    {
        if (rec.Countries != null)
        {
            study.Region = rec.Countries.Where(c => c.IdCountry?.Code != null).Select(c => new CodeableConcept("urn:iso:std:iso:3166", c.IdCountry!.Code, c.IdCountry.Code == "DE" ? "Germany" : c.IdCountry.Code)).ToList();
        }
    }

    private static void MapSites(DrksStudy drks, Recruitment rec, ResearchStudy study)
    {
        if (rec.Institutes != null)
        {
            study.Contained ??= [];
            study.Site = [];

            for (int i = 0; i < rec.Institutes.Count; i++)
            {
                var inst = rec.Institutes[i];
                var locId = $"{drks.DrksId}-Recruitment-Institute-{i}";

                var (roleCode, roleDisplay, typeSystem) = inst.Type switch
                {
                    InstituteType.UNI_MEDICAL_CENTER => ("umc", "Uni Medical Center", "https://www.imise.uni-leipzig.de/fhir/CodeSystem/InstituteType"),
                    InstituteType.MEDICAL_CENTER => ("HOSP", "Hospital", "http://terminology.hl7.org/CodeSystem/v3-RoleCode"),
                    _ => ("OF", "Outpatient facility", "http://terminology.hl7.org/CodeSystem/v3-RoleCode")
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

        if (rec.Gender != null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept("http://loinc.org", "76691-5", "Gender identity"),
                Value = new CodeableConcept { Text = rec.Gender.ToString() },
                Exclude = false
            });
        }

        var ageChar = FhirHelper.BuildAgeCharacteristic(rec.MinimumAge, rec.MaximumAge);
        if (ageChar != null)
        {
            group.Characteristic.Add(ageChar);
        }            

        var enRecDesc = rec.RecruitmentDescriptions?.FirstOrDefault(d => d.IdLocale?.Locale == Locale.en);
        var deRecDesc = rec.RecruitmentDescriptions?.FirstOrDefault(d => d.IdLocale?.Locale == Locale.de);

        if (enRecDesc?.ExclusionCriteria != null || deRecDesc?.ExclusionCriteria != null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept { Text = "FREETEXT" },
                Value = new CodeableConcept
                {
                    TextElement = FhirHelper.BuildTranslatedString(enRecDesc?.ExclusionCriteria, deRecDesc?.ExclusionCriteria)
                },
                Exclude = true
            });
        }

        if (enRecDesc?.InclusionCriteria != null || deRecDesc?.InclusionCriteria != null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept { Text = "FREETEXT" },
                Value = new CodeableConcept
                {
                    TextElement = FhirHelper.BuildTranslatedString(enRecDesc?.InclusionCriteria, deRecDesc?.InclusionCriteria)
                },
                Exclude = false
            });
        }

        if (rec.HealthyVolunteers != null)
        {
            group.Characteristic.Add(new Group.CharacteristicComponent
            {
                Code = new CodeableConcept("https://www.imise.uni-leipzig.de/fhir/CodeSystem/EligibilityCriteria", "healthyVolunteers", "Healthy volunteers"),
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
    private static void MapEligibilityReverse(Group group, Recruitment r)
    {
        foreach (var ch in group.Characteristic)
        {
            var code = ch.Code?.Coding.FirstOrDefault()?.Code;
            var codeText = ch.Code?.Text;

            if (code == "76691-5")
            {
                var genderText = (ch.Value as CodeableConcept)?.Text;
                if (genderText != null && Enum.TryParse<Gender>(genderText, out var gender))
                {
                    r.Gender = gender;
                }                    
            }
            else if (code == "30525-0")
            {
                var range = ch.Value as Hl7.Fhir.Model.Range;
                if (range?.Low?.Value != null)
                {
                    r.MinimumAge = new AgeDto
                    {
                        Amount = (int)range.Low.Value.Value,
                        Unit = MapUcumToAgeUnit(range.Low.Code)
                    };
                }
                
                if (range?.High?.Value != null)
                {
                    r.MaximumAge = new AgeDto
                    {
                        Amount = (int)range.High.Value.Value,
                        Unit = MapUcumToAgeUnit(range.High.Code)
                    };
                }                    
                else
                {
                    r.MaximumAge = new AgeDto { Unit = AgeUnit.NO_MAX_AGE };
                }                    
            }
            else if (code == "healthyVolunteers")
            {
                r.HealthyVolunteers = (ch.Value as FhirBoolean)?.Value;
            }
            else if (codeText == "FREETEXT")
            {
                var cc = ch.Value as CodeableConcept;
                var enText = cc?.Text;
                var deText = TitleDescriptionMapper.GetTranslation(cc?.TextElement);

                r.RecruitmentDescriptions ??= [];
                var enDesc = GetOrAddDescription(r, Locale.en);
                var deDesc = GetOrAddDescription(r, Locale.de);

                if (ch.Exclude == true)
                {
                    enDesc.ExclusionCriteria = enText;
                    deDesc.ExclusionCriteria = deText;
                }
                else
                {
                    enDesc.InclusionCriteria = enText;
                    deDesc.InclusionCriteria = deText;
                }
            }
        }
    }

    private static RecruitmentDescription GetOrAddDescription(Recruitment r, Locale locale)
    {
        var existing = r.RecruitmentDescriptions!.FirstOrDefault(d => d.IdLocale?.Locale == locale);
        if (existing != null)
        {
            return existing;
        }
        var desc = new RecruitmentDescription { IdLocale = new IdLocale { Locale = locale } };
        r.RecruitmentDescriptions!.Add(desc);
        return desc;
    }

    private static AgeUnit MapUcumToAgeUnit(string? code) => code switch
    {
        "d" => AgeUnit.DAYS,
        "wk" => AgeUnit.WEEKS,
        "mo" => AgeUnit.MONTHS,
        "a" => AgeUnit.YEARS,
        _ => AgeUnit.YEARS
    };
}