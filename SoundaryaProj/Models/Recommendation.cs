using System.Text.Json.Serialization;

namespace Models
{
    public class Recommendation
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("estimatedSavingKwh")]
        public double EstimatedSavingKwh { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; } = string.Empty;
    }
}
