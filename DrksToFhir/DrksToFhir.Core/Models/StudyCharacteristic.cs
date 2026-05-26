using DrksToFhir.Core.Enum;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class StudyCharacteristic
    {
        [JsonPropertyName("allocation")]
        public StudyAllocation? Allocation { get; set; }

        [JsonPropertyName("assignment")]
        public StudyAssignment? Assignment { get; set; }

        [JsonPropertyName("blindingType")]
        public BlindingType? BlindingType { get; set; }

        [JsonPropertyName("blindingWhoList")]
        public List<BlindingWhoItem>? BlindingWhoList { get; set; }

        [JsonPropertyName("characteristicDescriptions")]
        public List<CharacteristicDescription>? CharacteristicDescriptions { get; set; }

        [JsonPropertyName("controlTypes")]
        public List<ControlTypeItem>? ControlTypes { get; set; }

        [JsonPropertyName("duration")]
        public StudyDuration? Duration { get; set; }

        [JsonPropertyName("phase")]
        public StudyPhase? Phase { get; set; }

        [JsonPropertyName("purpose")]
        public StudyPurpose? Purpose { get; set; }

        [JsonPropertyName("timing")]
        public StudyTiming? Timing { get; set; }

        [JsonPropertyName("type")]
        public StudyType? Type { get; set; }
    }

    public class BlindingWhoItem
    {
        [JsonPropertyName("idBlindingWho")]
        public IdBlindingWho? IdBlindingWho { get; set; }
    }

    public class IdBlindingWho
    {
        [JsonPropertyName("blindingWho")]
        public BlindingWho? BlindingWho { get; set; }
    }

    public class CharacteristicDescription
    {
        [JsonPropertyName("idLocale")]
        public IdLocale? IdLocale { get; set; }

        [JsonPropertyName("primaryOutcome")]
        public string? PrimaryOutcome { get; set; }

        [JsonPropertyName("secondaryOutcome")]
        public string? SecondaryOutcome { get; set; }

        [JsonPropertyName("typeOfHiddenAllocation")]
        public string? TypeOfHiddenAllocation { get; set; }

        [JsonPropertyName("typeOfSequenceGeneration")]
        public string? TypeOfSequenceGeneration { get; set; }
    }

    public class ControlTypeItem
    {
        [JsonPropertyName("idControlType")]
        public IdControlType? IdControlType { get; set; }
    }

    public class IdControlType
    {
        [JsonPropertyName("controlType")]
        public ControlType? ControlType { get; set; }
    }
}