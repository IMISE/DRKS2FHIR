using DrksToFhir.Core.Enums;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class MaterialSupport
    {
        [JsonPropertyName("contact")]
        public DrksContact? Contact { get; set; }

        [JsonPropertyName("type")]
        public MaterialSupportType? Type { get; set; }
    }
}
