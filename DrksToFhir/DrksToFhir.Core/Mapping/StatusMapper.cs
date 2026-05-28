using DrksToFhir.Core.Enums;
using DrksToFhir.Core.Models;
using Hl7.Fhir.Model;

namespace DrksToFhir.Core.Mapping;

internal static class StatusMapper
{
    internal static void Map(DrksStudy drks, ResearchStudy study)
    {
        study.Status = drks.TrialStatus switch
        {
            TrialStatus.REGISTERED or TrialStatus.UPDATED => PublicationStatus.Active,
            TrialStatus.IN_PROCESSING or TrialStatus.REGISTRATION_REQUESTED
                or TrialStatus.UPDATE_REQUESTED => PublicationStatus.Draft,
            TrialStatus.RETURNED => PublicationStatus.Draft,
            _ => PublicationStatus.Retired
        };

        study.ProgressStatus = [];

        if (drks.RegistrationDrks is not null)
        {
            study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
            {
                State = new CodeableConcept(
                    "http://hl7.org/fhir/research-study-status", "approved", "Approved"),
                Actual = false,
                Period = new Period
                {
                    StartElement = new FhirDateTime(drks.RegistrationDrks),
                    EndElement = new FhirDateTime(drks.RegistrationDrks)
                }
            });
        }

        if (drks.Recruitment?.Status is null) return;

        var (code, display) = drks.Recruitment.Status switch
        {
            RecruitingStatus.RECRUITING => ("recruiting", "Recruiting"),
            RecruitingStatus.PENDING => ("approved", "Approved"),
            RecruitingStatus.SUSPENDED => ("temporarily-closed-to-accrual", "Temporarily closed to accrual"),
            RecruitingStatus.COMPLETE_FOLLOW_UP_CONTINUING => ("active", "Active"),
            RecruitingStatus.COMPLETE_FOLLOW_UP_COMPLETE => ("closed-to-accrual-and-intervention", "Closed to accrual and intervention"),
            RecruitingStatus.DISCONTINIUED => ("withdrawn", "Withdrawn"),
            RecruitingStatus.WITHDRAWN => ("withdrawn", "Withdrawn"),
            RecruitingStatus.INVITE_ONLY => ("recruiting", "Recruiting"),
            _ => ("unknown", "Unknown")
        };

        study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
        {
            State = new CodeableConcept(
                "http://hl7.org/fhir/research-study-status", code, display)
        });

        if (drks.Recruitment.ActualStartDate is not null)
        {
            study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
            {
                State = new CodeableConcept(
                    "http://hl7.org/fhir/research-study-status", code, display),
                Actual = true,
                Period = new Period
                {
                    StartElement = new FhirDateTime(drks.Recruitment.ActualStartDate),
                    EndElement = drks.Recruitment.ActualCompletionDate is not null
                        ? new FhirDateTime(drks.Recruitment.ActualCompletionDate)
                        : null
                }
            });
        }

        if (drks.Recruitment.ScheduledCompletionDate is not null)
        {
            study.ProgressStatus.Add(new ResearchStudy.ProgressStatusComponent
            {
                State = new CodeableConcept(
                    "http://hl7.org/fhir/research-study-status", code, display),
                Actual = false,
                Period = new Period
                {
                    StartElement = drks.Recruitment.ScheduledStartDate is not null
                        ? new FhirDateTime(drks.Recruitment.ScheduledStartDate)
                        : null,
                    EndElement = new FhirDateTime(drks.Recruitment.ScheduledCompletionDate)
                }
            });
        }
    }
}