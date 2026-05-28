using DrksToFhir.Core.Enums;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class TrialResults
    {
        [JsonPropertyName("firstPublication")]
        public string? FirstPublication { get; set; }

        [JsonPropertyName("firstResultPublication")]
        public string? FirstResultPublication { get; set; }

        [JsonPropertyName("ipdSharingPlan")]
        public bool? IpdSharingPlan { get; set; }

        [JsonPropertyName("plannedPublication")]
        public string? PlannedPublication { get; set; }

        [JsonPropertyName("publications")]
        public List<Publication>? Publications { get; set; }

        [JsonPropertyName("relatedDrksTrials")]
        public List<RelatedDrksTrial>? RelatedDrksTrials { get; set; }

        [JsonPropertyName("trialResultsDescriptions")]
        public List<TrialResultsDescription>? TrialResultsDescriptions { get; set; }
    }

    public class Publication
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("type")]
        public PublicationType? Type { get; set; }
    }

    public class RelatedDrksTrial
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("drksId")]
        public string? DrksId { get; set; }
    }

    public class TrialResultsDescription
    {
        [JsonPropertyName("briefSummaryOfResultsDescription")]
        public string? BriefSummaryOfResultsDescription { get; set; }

        [JsonPropertyName("idLocale")]
        public IdLocale? IdLocale { get; set; }

        [JsonPropertyName("ipdSharingPlanDescription")]
        public string? IpdSharingPlanDescription { get; set; }
    }
}
