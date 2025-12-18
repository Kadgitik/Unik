using Microsoft.ML.Data;

namespace WineRecommendationApp.Models
{
    public class WinePrediction
    {
        [ColumnName("Score")]
        public float Quality { get; set; }
    }
}