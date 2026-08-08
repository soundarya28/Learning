using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System;

namespace Models
{
    public class Consumption
    {
        [JsonPropertyName("applianceId")]
        public int ApplianceId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("hoursUsed")]
        public double HoursUsed { get; set; }
    }
}
