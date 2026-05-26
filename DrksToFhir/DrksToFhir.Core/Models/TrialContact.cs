using DrksToFhir.Core.Enum;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class TrialContact
    {
        [JsonPropertyName("contact")]
        public DrksContact? Contact { get; set; }

        [JsonPropertyName("idContactIdType")]
        public IdContactIdType? IdContactIdType { get; set; }

        [JsonPropertyName("otherType")]
        public OtherContactType? OtherType { get; set; }
    }

    public class IdContactIdType
    {
        [JsonPropertyName("type")]
        public TrialContactType? Type { get; set; }
    }
}
