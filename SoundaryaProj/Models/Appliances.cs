using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class Appliances
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("ratedPowerWatts")]
        public int RatedPowerWatts { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
