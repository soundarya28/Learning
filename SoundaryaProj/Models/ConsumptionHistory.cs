using System;
using System.Text.Json.Serialization;

namespace Models
{
    public class ConsumptionHistory
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("appliance")]
        public string Appliance { get; set; } = string.Empty;

        [JsonPropertyName("energyKwh")]
        public double EnergyKwh { get; set; }
    }
}
