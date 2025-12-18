using Microsoft.AspNetCore.Mvc;
using WineRecommendationApp.Models;
using WineRecommendationApp.Services;

namespace WineRecommendationApp.Controllers
{
    public class WineController : Controller
    {
        private readonly MLModelEngine _mlEngine;

        public WineController(MLModelEngine mlEngine)
        {
            _mlEngine = mlEngine;
        }

        // GET: /Wine
        public IActionResult Index()
        {
            return View(new UserPreferences());
        }

        // POST: /Wine/GetRecommendations
        [HttpPost]
        public IActionResult GetRecommendations(UserPreferences preferences)
        {
            var recommendations = new List<WineRecommendation>();

            // === ЧЕРВОНІ ВИНА ===

            // 1. Елітне Каберне Совіньйон
            if (preferences.WineType == "red" || preferences.WineType == "all")
            {
                var wine1 = new WineData
                {
                    FixedAcidity = 8.1f,
                    VolatileAcidity = 0.56f,
                    CitricAcid = 0.28f,
                    ResidualSugar = 2.1f,
                    Chlorides = 0.075f,
                    FreeSulfurDioxide = 17f,
                    TotalSulfurDioxide = 60f,
                    Density = 0.9980f,
                    PH = preferences.PreferredPH,
                    Sulphates = 0.62f,
                    Alcohol = Math.Min(preferences.MaxAlcohol, 13.5f)
                };
                var pred1 = _mlEngine.Predict(wine1);
                recommendations.Add(new WineRecommendation
                {
                    Id = 1,
                    Name = "Каберне Совіньйон Резерв",
                    Type = "Red",
                    PredictedQuality = pred1.Quality,
                    Alcohol = wine1.Alcohol,
                    PH = wine1.PH,
                    ResidualSugar = wine1.ResidualSugar,
                    Price = 450,
                    MatchScore = CalculateMatchScore(preferences, wine1),
                    Description = "Елегантне червоне вино з танінами дуба та нотками чорної смородини"
                });
            }

            // 2. Піно Нуар преміум
            if (preferences.WineType == "red" || preferences.WineType == "all")
            {
                var wine2 = new WineData
                {
                    FixedAcidity = 7.4f,
                    VolatileAcidity = 0.66f,
                    CitricAcid = 0.0f,
                    ResidualSugar = 1.8f,
                    Chlorides = 0.075f,
                    FreeSulfurDioxide = 13f,
                    TotalSulfurDioxide = 40f,
                    Density = 0.9978f,
                    PH = preferences.PreferredPH + 0.1f,
                    Sulphates = 0.60f,
                    Alcohol = (preferences.MinAlcohol + preferences.MaxAlcohol) / 2
                };
                var pred2 = _mlEngine.Predict(wine2);
                recommendations.Add(new WineRecommendation
                {
                    Id = 2,
                    Name = "Піно Нуар Елітний",
                    Type = "Red",
                    PredictedQuality = pred2.Quality,
                    Alcohol = wine2.Alcohol,
                    PH = wine2.PH,
                    ResidualSugar = wine2.ResidualSugar,
                    Price = 520,
                    MatchScore = CalculateMatchScore(preferences, wine2),
                    Description = "Витончене вино з ароматами вишні та м'якими танінами"
                });
            }

            // 3. Мерло класичне
            if (preferences.WineType == "red" || preferences.WineType == "all")
            {
                var wine3 = new WineData
                {
                    FixedAcidity = 7.8f,
                    VolatileAcidity = 0.58f,
                    CitricAcid = 0.02f,
                    ResidualSugar = 2.0f,
                    Chlorides = 0.073f,
                    FreeSulfurDioxide = 9f,
                    TotalSulfurDioxide = 18f,
                    Density = 0.9968f,
                    PH = preferences.PreferredPH - 0.1f,
                    Sulphates = 0.65f,
                    Alcohol = preferences.MinAlcohol + 1.5f
                };
                var pred3 = _mlEngine.Predict(wine3);
                recommendations.Add(new WineRecommendation
                {
                    Id = 3,
                    Name = "Мерло Класик",
                    Type = "Red",
                    PredictedQuality = pred3.Quality,
                    Alcohol = wine3.Alcohol,
                    PH = wine3.PH,
                    ResidualSugar = wine3.ResidualSugar,
                    Price = 350,
                    MatchScore = CalculateMatchScore(preferences, wine3),
                    Description = "М'яке червоне вино з фруктовими нотками та шовковистою текстурою"
                });
            }

            // 4. Сіра преміальний
            if (preferences.WineType == "red" || preferences.WineType == "all")
            {
                var wine4 = new WineData
                {
                    FixedAcidity = 8.5f,
                    VolatileAcidity = 0.52f,
                    CitricAcid = 0.3f,
                    ResidualSugar = 2.5f,
                    Chlorides = 0.08f,
                    FreeSulfurDioxide = 15f,
                    TotalSulfurDioxide = 45f,
                    Density = 0.9988f,
                    PH = preferences.PreferredPH + 0.2f,
                    Sulphates = 0.68f,
                    Alcohol = Math.Max(preferences.MinAlcohol, 12.0f)
                };
                var pred4 = _mlEngine.Predict(wine4);
                recommendations.Add(new WineRecommendation
                {
                    Id = 4,
                    Name = "Сіра Гранд Резерв",
                    Type = "Red",
                    PredictedQuality = pred4.Quality,
                    Alcohol = wine4.Alcohol,
                    PH = wine4.PH,
                    ResidualSugar = wine4.ResidualSugar,
                    Price = 480,
                    MatchScore = CalculateMatchScore(preferences, wine4),
                    Description = "Потужне вино з пряними нотками та довгим післясмаком"
                });
            }

            // === БІЛІ ВИНА ===

            // 5. Шардоне витримане
            if (preferences.WineType == "white" || preferences.WineType == "all")
            {
                var wine5 = new WineData
                {
                    FixedAcidity = 7.0f,
                    VolatileAcidity = 0.27f,
                    CitricAcid = 0.36f,
                    ResidualSugar = preferences.Sweetness == "sweet" ? 18.0f : 6.0f,
                    Chlorides = 0.045f,
                    FreeSulfurDioxide = 45f,
                    TotalSulfurDioxide = 170f,
                    Density = 1.001f,
                    PH = 3.0f,
                    Sulphates = 0.45f,
                    Alcohol = preferences.MaxAlcohol - 0.5f
                };
                var pred5 = _mlEngine.Predict(wine5);
                recommendations.Add(new WineRecommendation
                {
                    Id = 5,
                    Name = "Шардоне Преміум",
                    Type = "White",
                    PredictedQuality = pred5.Quality,
                    Alcohol = wine5.Alcohol,
                    PH = wine5.PH,
                    ResidualSugar = wine5.ResidualSugar,
                    Price = 420,
                    MatchScore = CalculateMatchScore(preferences, wine5),
                    Description = "Елегантне біле вино з ароматами тропічних фруктів та ванілі"
                });
            }

            // 6. Совіньйон Блан
            if (preferences.WineType == "white" || preferences.WineType == "all")
            {
                var wine6 = new WineData
                {
                    FixedAcidity = 6.8f,
                    VolatileAcidity = 0.26f,
                    CitricAcid = 0.44f,
                    ResidualSugar = 5.2f,
                    Chlorides = 0.040f,
                    FreeSulfurDioxide = 53f,
                    TotalSulfurDioxide = 170f,
                    Density = 0.9940f,
                    PH = 3.15f,
                    Sulphates = 0.56f,
                    Alcohol = (preferences.MinAlcohol + preferences.MaxAlcohol) / 2
                };
                var pred6 = _mlEngine.Predict(wine6);
                recommendations.Add(new WineRecommendation
                {
                    Id = 6,
                    Name = "Совіньйон Блан",
                    Type = "White",
                    PredictedQuality = pred6.Quality,
                    Alcohol = wine6.Alcohol,
                    PH = wine6.PH,
                    ResidualSugar = wine6.ResidualSugar,
                    Price = 380,
                    MatchScore = CalculateMatchScore(preferences, wine6),
                    Description = "Свіже біле вино з нотками крижу та цитрусових"
                });
            }

            // 7. Рислінг напівсолодкий
            if (preferences.WineType == "white" || preferences.WineType == "all")
            {
                var wine7 = new WineData
                {
                    FixedAcidity = 7.5f,
                    VolatileAcidity = 0.23f,
                    CitricAcid = 0.49f,
                    ResidualSugar = preferences.Sweetness == "sweet" ? 25.0f : 12.0f,
                    Chlorides = 0.038f,
                    FreeSulfurDioxide = 65f,
                    TotalSulfurDioxide = 210f,
                    Density = 1.005f,
                    PH = 2.95f,
                    Sulphates = 0.52f,
                    Alcohol = preferences.MinAlcohol + 1.0f
                };
                var pred7 = _mlEngine.Predict(wine7);
                recommendations.Add(new WineRecommendation
                {
                    Id = 7,
                    Name = "Рислінг Спатлезе",
                    Type = "White",
                    PredictedQuality = pred7.Quality,
                    Alcohol = wine7.Alcohol,
                    PH = wine7.PH,
                    ResidualSugar = wine7.ResidualSugar,
                    Price = 440,
                    MatchScore = CalculateMatchScore(preferences, wine7),
                    Description = "Напівсолодке німецьке вино з медовими та квітковими тонами"
                });
            }

            // 8. Піно Гріджо
            if (preferences.WineType == "white" || preferences.WineType == "all")
            {
                var wine8 = new WineData
                {
                    FixedAcidity = 6.6f,
                    VolatileAcidity = 0.24f,
                    CitricAcid = 0.28f,
                    ResidualSugar = 3.8f,
                    Chlorides = 0.042f,
                    FreeSulfurDioxide = 41f,
                    TotalSulfurDioxide = 132f,
                    Density = 0.9920f,
                    PH = 3.22f,
                    Sulphates = 0.48f,
                    Alcohol = preferences.MaxAlcohol - 1.5f
                };
                var pred8 = _mlEngine.Predict(wine8);
                recommendations.Add(new WineRecommendation
                {
                    Id = 8,
                    Name = "Піно Гріджо Італійське",
                    Type = "White",
                    PredictedQuality = pred8.Quality,
                    Alcohol = wine8.Alcohol,
                    PH = wine8.PH,
                    ResidualSugar = wine8.ResidualSugar,
                    Price = 320,
                    MatchScore = CalculateMatchScore(preferences, wine8),
                    Description = "Легке італійське вино з мінеральними нотками та свіжістю"
                });
            }

            // 9. Гевюрцтрамінер
            if (preferences.WineType == "white" || preferences.WineType == "all")
            {
                var wine9 = new WineData
                {
                    FixedAcidity = 7.2f,
                    VolatileAcidity = 0.29f,
                    CitricAcid = 0.38f,
                    ResidualSugar = preferences.Sweetness == "sweet" ? 22.0f : 9.5f,
                    Chlorides = 0.046f,
                    FreeSulfurDioxide = 58f,
                    TotalSulfurDioxide = 195f,
                    Density = 1.002f,
                    PH = 3.08f,
                    Sulphates = 0.49f,
                    Alcohol = preferences.MinAlcohol + 2.0f
                };
                var pred9 = _mlEngine.Predict(wine9);
                recommendations.Add(new WineRecommendation
                {
                    Id = 9,
                    Name = "Гевюрцтрамінер",
                    Type = "White",
                    PredictedQuality = pred9.Quality,
                    Alcohol = wine9.Alcohol,
                    PH = wine9.PH,
                    ResidualSugar = wine9.ResidualSugar,
                    Price = 460,
                    MatchScore = CalculateMatchScore(preferences, wine9),
                    Description = "Ароматне вино з нотками личі, троянди та пряних спецій"
                });
            }

            // 10. Шенен Блан
            if (preferences.WineType == "white" || preferences.WineType == "all")
            {
                var wine10 = new WineData
                {
                    FixedAcidity = 6.9f,
                    VolatileAcidity = 0.25f,
                    CitricAcid = 0.42f,
                    ResidualSugar = 7.5f,
                    Chlorides = 0.043f,
                    FreeSulfurDioxide = 48f,
                    TotalSulfurDioxide = 155f,
                    Density = 0.9950f,
                    PH = 3.18f,
                    Sulphates = 0.51f,
                    Alcohol = (preferences.MinAlcohol + preferences.MaxAlcohol) / 2
                };
                var pred10 = _mlEngine.Predict(wine10);
                recommendations.Add(new WineRecommendation
                {
                    Id = 10,
                    Name = "Шенен Блан Луара",
                    Type = "White",
                    PredictedQuality = pred10.Quality,
                    Alcohol = wine10.Alcohol,
                    PH = wine10.PH,
                    ResidualSugar = wine10.ResidualSugar,
                    Price = 390,
                    MatchScore = CalculateMatchScore(preferences, wine10),
                    Description = "Універсальне французьке вино з яблучними та медовими нотами"
                });
            }

            // Фільтрація за ціною та сортування
            var filteredRecommendations = recommendations
                .Where(r => r.Price <= preferences.MaxPrice)
                .OrderByDescending(r => r.PredictedQuality)
                .ThenByDescending(r => r.MatchScore)
                .Take(6)
                .ToList();

            return View("Recommendations", filteredRecommendations);
        }

        // Розрахунок відповідності вподобанням
        private int CalculateMatchScore(UserPreferences prefs, WineData wine)
        {
            int score = 70; // Базовий рахунок

            // Алкоголь (+15 балів)
            if (wine.Alcohol >= prefs.MinAlcohol && wine.Alcohol <= prefs.MaxAlcohol)
                score += 15;
            else
                score -= 5;

            // pH (+10 балів)
            float phDiff = Math.Abs(wine.PH - prefs.PreferredPH);
            if (phDiff < 0.2f)
                score += 10;
            else if (phDiff < 0.5f)
                score += 5;

            // Солодкість (+5 балів)
            if (prefs.Sweetness == "sweet" && wine.ResidualSugar > 15)
                score += 5;
            else if (prefs.Sweetness == "medium" && wine.ResidualSugar > 5 && wine.ResidualSugar < 15)
                score += 5;
            else if (prefs.Sweetness == "dry" && wine.ResidualSugar < 5)
                score += 5;

            return Math.Min(100, Math.Max(50, score));
        }
    }
}