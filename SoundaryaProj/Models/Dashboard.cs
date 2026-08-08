using System;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace Models
{
    public class Dashboard
    {
        [JsonPropertyName("todayConsumptionKwh")]
        public double TodayConsumptionKwh { get; set; }

        [JsonPropertyName("weeklyConsumptionKwh")]
        public double WeeklyConsumptionKwh { get; set; }

        [JsonPropertyName("monthlyConsumptionKwh")]
        public double MonthlyConsumptionKwh { get; set; }

        [JsonPropertyName("predictedNextWeekKwh")]
        public double PredictedNextWeekKwh { get; set; }

        [JsonPropertyName("estimatedMonthlyBill")]
        public decimal EstimatedMonthlyBill { get; set; }

        [JsonPropertyName("topConsumer")]
        public TopConsumerInfo TopConsumer { get; set; }
    }

    public class TopConsumerInfo
    {
        [JsonPropertyName("appliance")]
        public string Appliance { get; set; }

        [JsonPropertyName("percentage")]
        public int Percentage { get; set; }
    }
}
