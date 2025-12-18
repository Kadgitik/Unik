using Microsoft.ML;
using Microsoft.ML.Data;
using WineRecommendationApp.Models;

namespace WineRecommendationApp.Services
{
    public class MLModelEngine
    {
        private readonly MLContext _mlContext;
        private ITransformer? _model;
        private readonly string _modelPath = Path.Combine("MLModels", "wine_model.zip");

        public MLModelEngine()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void TrainModel(string dataPath)
{
    Console.WriteLine("\n🍷 ===== ТРЕНУВАННЯ ПОКРАЩЕНОЇ ML МОДЕЛІ =====\n");
    Console.WriteLine("🔄 Завантаження даних...");

    // 1. Завантажити дані
    IDataView dataView = _mlContext.Data.LoadFromTextFile<WineData>(
        path: dataPath,
        hasHeader: true,
        separatorChar: ';');

    Console.WriteLine("✅ Дані завантажено з CSV файлу");

    // 2. Розділити на train/test (80/20)
    var splitData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
    Console.WriteLine("📊 Дані розділено на Train (80%) та Test (20%)");

    // 3. ПОКРАЩЕНИЙ PIPELINE
    Console.WriteLine("🔧 Створення покращеного pipeline...");
    
    var pipeline = _mlContext.Transforms
        // Об'єднання features
        .Concatenate(
            "Features",
            nameof(WineData.FixedAcidity),
            nameof(WineData.VolatileAcidity),
            nameof(WineData.CitricAcid),
            nameof(WineData.ResidualSugar),
            nameof(WineData.Chlorides),
            nameof(WineData.FreeSulfurDioxide),
            nameof(WineData.TotalSulfurDioxide),
            nameof(WineData.Density),
            nameof(WineData.PH),
            nameof(WineData.Sulphates),
            nameof(WineData.Alcohol))
        // Нормалізація features
        .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
        // ПОКРАЩЕНІ параметри FastTree
        .Append(_mlContext.Regression.Trainers.FastTree(
            labelColumnName: "Label",
            featureColumnName: "Features",
            numberOfLeaves: 30,              // ↑ збільшено з 20
            numberOfTrees: 200,              // ↑ збільшено зі 100
            minimumExampleCountPerLeaf: 5,   // ↓ зменшено з 10
            learningRate: 0.15));            // ↓ зменшено з 0.2

    Console.WriteLine("🔄 Тренування моделі FastTree...");
    Console.WriteLine("⚙️  Параметри:");
    Console.WriteLine("   • Дерев: 200 (було 100)");
    Console.WriteLine("   • Листків: 30 (було 20)");
    Console.WriteLine("   • Learning Rate: 0.15 (було 0.2)");
    Console.WriteLine("   • Min Examples: 5 (було 10)");
    Console.WriteLine("   • Нормалізація: MinMax");
    Console.WriteLine("⏳ Це може зайняти 20-40 секунд...\n");

    // 4. Тренування моделі
    var startTime = DateTime.Now;
    _model = pipeline.Fit(splitData.TrainSet);
    var trainingTime = (DateTime.Now - startTime).TotalSeconds;

    Console.WriteLine($"✅ Модель натренована за {trainingTime:F1} секунд!\n");

    // 5. Оцінка моделі
    var predictions = _model.Transform(splitData.TestSet);
    var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

    Console.WriteLine("📊 ===== МЕТРИКИ ПОКРАЩЕНОЇ МОДЕЛІ =====");
    Console.WriteLine($"R² (R-Squared):        {metrics.RSquared:F4} ({metrics.RSquared * 100:F1}%)");
    Console.WriteLine($"MAE (Mean Abs Error):  {metrics.MeanAbsoluteError:F4}");
    Console.WriteLine($"RMSE (Root Mean Sq):   {metrics.RootMeanSquaredError:F4}");
    Console.WriteLine($"RMS (Loss Function):   {metrics.LossFunction:F4}");
    Console.WriteLine("==========================================\n");

    // Порівняння з базовою моделлю
    Console.WriteLine("📈 Порівняння з базовою моделлю:");
    Console.WriteLine("   Базова модель:     R² ≈ 0.45");
    Console.WriteLine($"   Покращена модель:  R² = {metrics.RSquared:F4}");
    
    double improvement = (metrics.RSquared - 0.45) / 0.45 * 100;
    if (improvement > 0)
        Console.WriteLine($"   Покращення:        +{improvement:F1}%\n");
    else
        Console.WriteLine($"   Різниця:           {improvement:F1}%\n");

    // Оцінка якості
    if (metrics.RSquared > 0.55)
        Console.WriteLine("🎉 ВІДМІННА точність моделі!");
    else if (metrics.RSquared > 0.48)
        Console.WriteLine("✅ Модель показує ХОРОШУ точність!");
    else if (metrics.RSquared > 0.40)
        Console.WriteLine("⚠️  Модель показує прийнятну точність");
    else
        Console.WriteLine("❌ Модель потребує значного покращення");

    // 6. Збереження моделі
    Directory.CreateDirectory("MLModels");
    _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
    
    var fileSize = new FileInfo(_modelPath).Length / 1024;
    Console.WriteLine($"\n💾 Модель збережена: {_modelPath}");
    Console.WriteLine($"📦 Розмір файлу: {fileSize} KB");
    Console.WriteLine($"⏱️  Час тренування: {trainingTime:F1}s\n");
}

        public void LoadModel()
        {
            if (File.Exists(_modelPath))
            {
                _model = _mlContext.Model.Load(_modelPath, out var modelSchema);
                Console.WriteLine($"✅ ML модель завантажена з {_modelPath}");
            }
            else
            {
                throw new FileNotFoundException($"❌ Модель не знайдена: {_modelPath}");
            }
        }

        public WinePrediction Predict(WineData wineData)
        {
            if (_model == null)
                LoadModel();

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<WineData, WinePrediction>(_model!);
            return predictionEngine.Predict(wineData);
        }
    }
}