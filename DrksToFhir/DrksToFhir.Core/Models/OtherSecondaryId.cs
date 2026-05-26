using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class OtherSecondaryId
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
