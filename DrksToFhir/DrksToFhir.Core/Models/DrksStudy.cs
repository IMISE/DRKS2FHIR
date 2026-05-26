using DrksToFhir.Core.Enum;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class DrksStudy
    {
        [JsonPropertyName("drksId")]
        public string? DrksId { get; set; }

        [JsonPropertyName("ethicsCommittee")]
        public EthicsCommittee? EthicsCommittee { get; set; }

        [JsonPropertyName("lastUpdate")]
        public string? LastUpdate { get; set; }

        [JsonPropertyName("materialSupports")]
        public List<MaterialSupport>? MaterialSupports { get; set; }

        [JsonPropertyName("observationalGroups")]
        public List<ObservationalGroup>? ObservationalGroups { get; set; }

        [JsonPropertyName("otherSecondaryIds")]
        public List<OtherSecondaryId>? OtherSecondaryIds { get; set; }

        [JsonPropertyName("recruitment")]
        public Recruitment? Recruitment { get; set; }

        [JsonPropertyName("registrationDrks")]
        public string? RegistrationDrks { get; set; }

        [JsonPropertyName("registrationType")]
        public RegistrationType? RegistrationType { get; set; }

        [JsonPropertyName("secondaryIds")]
        public SecondaryIds? SecondaryIds { get; set; }

        [JsonPropertyName("sponsored")]
        public bool? Sponsored { get; set; }

        [JsonPropertyName("studiedHealthConditions")]
        public List<StudiedHealthCondition>? StudiedHealthConditions { get; set; }

        [JsonPropertyName("studyCharacteristic")]
        public StudyCharacteristic? StudyCharacteristic { get; set; }

        [JsonPropertyName("trialContacts")]
        public List<TrialContact>? TrialContacts { get; set; }

        [JsonPropertyName("trialDescriptions")]
        public List<TrialDescription>? TrialDescriptions { get; set; }

        [JsonPropertyName("trialResults")]
        public TrialResults? TrialResults { get; set; }

        [JsonPropertyName("trialStatus")]
        public TrialStatus? TrialStatus { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
