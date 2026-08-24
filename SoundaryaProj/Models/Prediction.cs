using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class Prediction
    {
        [JsonPropertyName("averageDailyConsumptionKwh")]
        public double AverageDailyConsumptionKwh { get; set; }

        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

        [JsonPropertyName("dayOfWeek")]
        public string DayOfWeek { get; set; } = string.Empty;
    }
}
