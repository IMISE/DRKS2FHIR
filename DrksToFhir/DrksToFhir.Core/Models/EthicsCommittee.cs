using DrksToFhir.Core.Enum;
using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class EthicsCommittee
    {
        [JsonPropertyName("applicationDate")]
        public string? ApplicationDate { get; set; }

        [JsonPropertyName("ethicVotes")]
        public List<EthicVote>? EthicVotes { get; set; }

        [JsonPropertyName("number")]
        public string? Number { get; set; }
    }

    public class EthicVote
    {
        [JsonPropertyName("contact")]
        public DrksContact? Contact { get; set; }

        [JsonPropertyName("dateOfVote")]
        public string? DateOfVote { get; set; }

        [JsonPropertyName("type")]
        public TrialContactType? Type { get; set; }

        [JsonPropertyName("vote")]
        public EthicVoteResult? Vote { get; set; }
    }
}