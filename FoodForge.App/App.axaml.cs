using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using FoodForge.App.ViewModels;
using FoodForge.App.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodForge.App;

public partial class App : Application
{
    private static IServiceProvider _serviceProvider = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        _serviceProvider = ConfigureServices();

        InitializeDatabase();
    }

    private static void InitializeDatabase()
    {
        using var dbContext = _serviceProvider
            .GetRequiredService<IDbContextFactory<FoodForgeDbContext>>()
            .CreateDbContext();

        var initializer = new DatabaseInitializer(dbContext);
        initializer.Initialize();
    }
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainWindowViewModel>();

        services.AddTransient<ProfileRepository>();
        services.AddTransient<DishRepository>();
        services.AddTransient<DishService>();
        services.AddTransient<IngredientRepository>();
        services.AddTransient<DishIngredientRepository>();
        services.AddTransient<RecipeStepRepository>();

        services.AddSingleton<IDialogService, DialogService>();

        services.AddDbContextFactory<FoodForgeDbContext>(options =>
        {
            options.UseSqlite(DatabasePaths.GetConnectionString())
                   .UseSnakeCaseNamingConvention();
        });

        return services.BuildServiceProvider();
    } 
}