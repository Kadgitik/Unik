using Microsoft.ML.Data;

namespace WineRecommendationApp.Models
{
    public class WineData
    {
        [LoadColumn(0)]
        public float FixedAcidity { get; set; }

        [LoadColumn(1)]
        public float VolatileAcidity { get; set; }

        [LoadColumn(2)]
        public float CitricAcid { get; set; }

        [LoadColumn(3)]
        public float ResidualSugar { get; set; }

        [LoadColumn(4)]
        public float Chlorides { get; set; }

        [LoadColumn(5)]
        public float FreeSulfurDioxide { get; set; }

        [LoadColumn(6)]
        public float TotalSulfurDioxide { get; set; }

        [LoadColumn(7)]
        public float Density { get; set; }

        [LoadColumn(8)]
        public float PH { get; set; }

        [LoadColumn(9)]
        public float Sulphates { get; set; }

        [LoadColumn(10)]
        public float Alcohol { get; set; }

        [LoadColumn(11)]
        [ColumnName("Label")]
        public float Quality { get; set; }
    }
}