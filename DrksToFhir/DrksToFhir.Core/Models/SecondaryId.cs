using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class SecondaryIds
    {
        [JsonPropertyName("eudamedNumber")]
        public string? EudamedNumber { get; set; }

        [JsonPropertyName("eudraCtNumber")]
        public string? EudraCtNumber { get; set; }

        [JsonPropertyName("noOtherIdentificationNumbersAvailable")]
        public bool? NoOtherIdentificationNumbersAvailable { get; set; }

        [JsonPropertyName("otherPrimaryRegisterId")]
        public string? OtherPrimaryRegisterId { get; set; }

        [JsonPropertyName("otherPrimaryRegisterName")]
        public string? OtherPrimaryRegisterName { get; set; }

        [JsonPropertyName("universalTrialNumber")]
        public string? UniversalTrialNumber { get; set; }
    }
}
