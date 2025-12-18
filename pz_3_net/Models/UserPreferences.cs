namespace WineRecommendationApp.Models
{
    public class UserPreferences
    {
        public string WineType { get; set; } = "red";
        public string Sweetness { get; set; } = "dry";
        public float MinAlcohol { get; set; } = 9.0f;
        public float MaxAlcohol { get; set; } = 14.0f;
        public float PreferredPH { get; set; } = 3.3f;
        public decimal MaxPrice { get; set; } = 500;
    }

    public class WineRecommendation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public float PredictedQuality { get; set; }
        public float Alcohol { get; set; }
        public float PH { get; set; }
        public float ResidualSugar { get; set; }
        public int MatchScore { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}