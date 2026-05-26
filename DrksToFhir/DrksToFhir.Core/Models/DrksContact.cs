using System.Text.Json.Serialization;

namespace DrksToFhir.Core.Models
{
    public class DrksContact
    {
        [JsonPropertyName("affiliation")]
        public string? Affiliation { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("fax")]
        public string? Fax { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("streetAndNo")]
        public string? StreetAndNo { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("zip")]
        public string? Zip { get; set; }
    }
}