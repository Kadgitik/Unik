using WineRecommendationApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Додати сервіси
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MLModelEngine>();

var app = builder.Build();

// ===== ТРЕНУВАННЯ ML МОДЕЛІ ПРИ СТАРТІ =====
Console.WriteLine("\n Wine Recommendation App - Starting...\n");

var mlEngine = app.Services.GetRequiredService<MLModelEngine>();
string modelPath = Path.Combine("MLModels", "wine_model.zip");

if (!File.Exists(modelPath))
{
    Console.WriteLine(" ПЕРШИЙ ЗАПУСК - ПОТРІБНО НАТРЕНУВАТИ МОДЕЛЬ\n");

    string redWinePath = Path.Combine("Data", "winequality-red.csv");
    string whiteWinePath = Path.Combine("Data", "winequality-white.csv");
    string combinedPath = Path.Combine("Data", "winequality-combined.csv");

    if (File.Exists(redWinePath) && File.Exists(whiteWinePath))
    {
        Console.WriteLine(" Об'єднання червоних та білих вин...");
        
        var redLines = File.ReadAllLines(redWinePath);
        var whiteLines = File.ReadAllLines(whiteWinePath).Skip(1);

        using (var writer = new StreamWriter(combinedPath))
        {
            foreach (var line in redLines)
                writer.WriteLine(line);
            foreach (var line in whiteLines)
                writer.WriteLine(line);
        }

        Console.WriteLine($" Об'єднано файлів: {redLines.Length + whiteLines.Count()} вин\n");

        mlEngine.TrainModel(combinedPath);
        
        Console.WriteLine(" Модель готова до використання!\n");
    }
    else
    {
        Console.WriteLine(" ПОМИЛКА: Файли не знайдено в папці Data/");
    }
}
else
{
    Console.WriteLine(" ML модель вже існує");
    mlEngine.LoadModel();
    Console.WriteLine();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine(" Web сервер запущено!");
Console.WriteLine(" Відкрий браузер\n");

app.Run();
