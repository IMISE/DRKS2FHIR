using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class StatusMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.Status = drks.Recruitment?.Status switch
        {
            RecruitingStatus.PENDING => PublicationStatus.Draft,
            RecruitingStatus.RECRUITING or RecruitingStatus.INVITE_ONLY or RecruitingStatus.SUSPENDED or RecruitingStatus.COMPLETE_FOLLOW_UP_CONTINUING => PublicationStatus.Active,
            RecruitingStatus.COMPLETE_FOLLOW_UP_COMPLETE or RecruitingStatus.DISCONTINIUED or RecruitingStatus.WITHDRAWN => PublicationStatus.Retired,
            _ => PublicationStatus.Unknown
        };

        study.ProgressStatus = [];

        if (drks.RegistrationDrks != null)
        {
            study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
            {
                State = new CodeableConcept("http://hl7.org/fhir/research-study-status", "approved", "Approved"),
                Actual = drks.RegistrationType == RegistrationType.PROSPECTIVE,
                Period = new Period
                {
                    StartElement = new FhirDateTime(drks.RegistrationDrks),
                    EndElement = new FhirDateTime(drks.RegistrationDrks)
                }
            });
        }

        if (drks.Recruitment?.Status != null)
        {
            var (code, display) = drks.Recruitment.Status switch
            {
                RecruitingStatus.RECRUITING => ("recruiting", "Recruiting"),
                RecruitingStatus.PENDING => ("pending", "Pending"),
                RecruitingStatus.SUSPENDED => ("suspended", "Suspended"),
                RecruitingStatus.COMPLETE_FOLLOW_UP_CONTINUING => ("complete-follow-up-continuing", "Complete follow up continuing"),
                RecruitingStatus.COMPLETE_FOLLOW_UP_COMPLETE => ("complete-follow-up-complete", "Complete follow up complete"),
                RecruitingStatus.DISCONTINIUED => ("discontiniued", "Discontiniued"),
                RecruitingStatus.WITHDRAWN => ("withdrawn", "Withdrawn"),
                RecruitingStatus.INVITE_ONLY => ("invite-only", "Invite only"),
                _ => ("unknown", "Unknown")
            };

            study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
            {
                State = new CodeableConcept("https://www.imise.uni-leipzig.de/fhir/CodeSystem/RecruitingStatus", code, display)
            });

            if (drks.Recruitment.ActualStartDate != null)
            {
                study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
                {
                    State = new CodeableConcept("https://www.imise.uni-leipzig.de/fhir/CodeSystem/RecruitingStatus", code, display),
                    Actual = true,
                    Period = new Period
                    {
                        StartElement = new FhirDateTime(drks.Recruitment.ActualStartDate),
                        EndElement = drks.Recruitment.ActualCompletionDate != null
                            ? new FhirDateTime(drks.Recruitment.ActualCompletionDate)
                            : null
                    }
                });
            }

            if (drks.Recruitment.ScheduledCompletionDate != null)
            {
                study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
                {
                    State = new CodeableConcept("https://www.imise.uni-leipzig.de/fhir/CodeSystem/RecruitingStatus", code, display),
                    Actual = false,
                    Period = new Period
                    {
                        StartElement = drks.Recruitment.ScheduledStartDate != null
                            ? new FhirDateTime(drks.Recruitment.ScheduledStartDate)
                            : null,
                        EndElement = new FhirDateTime(drks.Recruitment.ScheduledCompletionDate)
                    }
                });
            }
        }

        MapTrialStatus(drks, study);
    }

    internal static void MapReverse(ResearchStudy study, DrksStudy drks)
    {
        var trialStatusExt = study.GetExtension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-trial-status");
        if (trialStatusExt?.Value is Code trialStatusCode && Enum.TryParse<TrialStatus>(trialStatusCode.Value?.ToUpperInvariant().Replace('-', '_'), out var ts))
        {
            drks.TrialStatus = ts;
        }            

        var approvedEntry = study.ProgressStatus.FirstOrDefault(ps => ps.State?.Coding.Any(c => c.System == "http://hl7.org/fhir/research-study-status" && c.Code == "approved") == true);
        if (approvedEntry?.Period?.Start != null)
        {
            drks.RegistrationDrks = approvedEntry.Period.Start;
            drks.RegistrationType = (approvedEntry.Actual == null || approvedEntry.Actual.Value) ? RegistrationType.PROSPECTIVE : RegistrationType.RETROSPECTIVE;
        }            

        var recruitEntry = study.ProgressStatus.FirstOrDefault(ps => ps.State?.Coding.Any(c => c.System == "https://www.imise.uni-leipzig.de/fhir/CodeSystem/RecruitingStatus") == true && ps.Actual == null && ps.Period == null);

        if (recruitEntry?.State?.Coding.FirstOrDefault(c => c.System == "https://www.imise.uni-leipzig.de/fhir/CodeSystem/RecruitingStatus")?.Code is string recruitCode 
            && Enum.TryParse<RecruitingStatus>(recruitCode.ToUpperInvariant().Replace('-', '_'), out var rs))
        {
            drks.Recruitment ??= new Recruitment();
            drks.Recruitment.Status = rs;
        }

        var actualEntry = study.ProgressStatus.FirstOrDefault(ps => ps.Actual == true && ps.State?.Coding.Any(c => c.System == "https://www.imise.uni-leipzig.de/fhir/CodeSystem/RecruitingStatus") == true);
        if (actualEntry != null)
        {
            drks.Recruitment ??= new Recruitment();
            drks.Recruitment.ActualStartDate = actualEntry.Period?.Start;
            drks.Recruitment.ActualCompletionDate = actualEntry.Period?.End;
        }

        var scheduledEntry = study.ProgressStatus.FirstOrDefault(ps => ps.Actual == false && ps.Period?.End != null && ps.State?.Coding.Any(c => c.System == "https://www.imise.uni-leipzig.de/fhir/CodeSystem/RecruitingStatus") == true);
        if (scheduledEntry != null)
        {
            drks.Recruitment ??= new Recruitment();
            drks.Recruitment.ScheduledStartDate = scheduledEntry.Period?.Start;
            drks.Recruitment.ScheduledCompletionDate = scheduledEntry.Period?.End;
        }
    }

    private static void MapTrialStatus(DrksStudy drks, ResearchStudy study)
    {
        if (drks.TrialStatus != null)
        {
            study.Extension.Add(new Extension("https://www.imise.uni-leipzig.de/fhir/StructureDefinition/drks-trial-status", new Code(drks.TrialStatus.ToString()!.ToLowerInvariant().Replace("_", "-"))));
        }
    }
}