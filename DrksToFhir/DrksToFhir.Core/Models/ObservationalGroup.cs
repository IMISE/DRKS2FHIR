using DrksToFhir.Core.Enums;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class ObservationalGroup
    {
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("idArm")]
        public IdArm? IdArm { get; set; }
    }

    public class IdArm
    {
        [JsonPropertyName("arm")]
        public int? Arm { get; set; }

        [JsonPropertyName("locale")]
        public Locale? Locale { get; set; }
    }
}
