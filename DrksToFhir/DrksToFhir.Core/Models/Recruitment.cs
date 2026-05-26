using DrksToFhir.Core.Enum;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class Recruitment
    {
        [JsonPropertyName("actualCompletionDate")]
        public string? ActualCompletionDate { get; set; }

        [JsonPropertyName("actualStartDate")]
        public string? ActualStartDate { get; set; }

        [JsonPropertyName("centricType")]
        public CentricType? CentricType { get; set; }

        [JsonPropertyName("countries")]
        public List<RecruitmentCountry>? Countries { get; set; }

        [JsonPropertyName("gender")]
        public Gender? Gender { get; set; }

        [JsonPropertyName("healthyVolunteers")]
        public bool? HealthyVolunteers { get; set; }

        [JsonPropertyName("institutes")]
        public List<Institute>? Institutes { get; set; }

        [JsonPropertyName("maximumAge")]
        public AgeDto? MaximumAge { get; set; }

        [JsonPropertyName("minimumAge")]
        public AgeDto? MinimumAge { get; set; }

        [JsonPropertyName("plannedParticipants")]
        public int? PlannedParticipants { get; set; }

        [JsonPropertyName("recruitmentDescriptions")]
        public List<RecruitmentDescription>? RecruitmentDescriptions { get; set; }

        [JsonPropertyName("scheduledCompletionDate")]
        public string? ScheduledCompletionDate { get; set; }

        [JsonPropertyName("scheduledStartDate")]
        public string? ScheduledStartDate { get; set; }

        [JsonPropertyName("status")]
        public RecruitingStatus? Status { get; set; }

        [JsonPropertyName("totalParticipants")]
        public int? TotalParticipants { get; set; }
    }

    public class RecruitmentCountry
    {
        [JsonPropertyName("idCountry")]
        public IdCountry? IdCountry { get; set; }
    }

    public class IdCountry
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    public class Institute
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public InstituteType? Type { get; set; }
    }

    public class AgeDto
    {
        [JsonPropertyName("amount")]
        public int? Amount { get; set; }

        [JsonPropertyName("unit")]
        public AgeUnit? Unit { get; set; }
    }

    public class RecruitmentDescription
    {
        [JsonPropertyName("exclusionCriteria")]
        public string? ExclusionCriteria { get; set; }

        [JsonPropertyName("idLocale")]
        public IdLocale? IdLocale { get; set; }

        [JsonPropertyName("inclusionCriteria")]
        public string? InclusionCriteria { get; set; }

        [JsonPropertyName("reasonDescription")]
        public string? ReasonDescription { get; set; }
    }
}