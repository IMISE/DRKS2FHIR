using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class TrialDescription
    {
        [JsonPropertyName("acronym")]
        public string? Acronym { get; set; }

        [JsonPropertyName("idLocale")]
        public IdLocale? IdLocale { get; set; }

        [JsonPropertyName("scientificSummary")]
        public string? ScientificSummary { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
