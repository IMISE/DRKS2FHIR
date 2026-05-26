using DrksToFhir.Core.Enum;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class IdLocale
    {
        [JsonPropertyName("locale")]
        public Locale Locale { get; set; }
    }
}
