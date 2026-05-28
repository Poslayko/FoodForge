using Avalonia;
using Microsoft.EntityFrameworkCore;

namespace FoodForge.App;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        InitializeDatabase();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    } 

    private static void InitializeDatabase()
    {
        var connectionString = DatabasePaths.GetConnectionString();

        var options = new DbContextOptionsBuilder<FoodForgeDbContext>()
            .UseSqlite(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        using var dbContext = new FoodForgeDbContext(options);

        var initializer = new DatabaseInitializer(dbContext);
        initializer.Initialize();
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
