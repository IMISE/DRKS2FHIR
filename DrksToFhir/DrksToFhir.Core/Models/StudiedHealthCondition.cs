using DrksToFhir.Core.Enums;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class StudiedHealthCondition
    {
        [JsonPropertyName("catalog")]
        public HealthConditionCatalog? Catalog { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("studiedHealthConditionDescriptions")]
        public List<StudiedHealthConditionDescription>? StudiedHealthConditionDescriptions { get; set; }

        [JsonPropertyName("type")]
        public HealthConditionType? Type { get; set; }
    }

    public class StudiedHealthConditionDescription
    {
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("idLocale")]
        public IdLocale? IdLocale { get; set; }
    }   
}
