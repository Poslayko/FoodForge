using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using FoodForge.App.ViewModels;
using FoodForge.App.Services;

namespace FoodForge.App;

public partial class App : Application
{
    private IServiceProvider _serviceProvider = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        _serviceProvider = ConfigureServices();
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
        services.AddTransient<DishIngredientRepository>();
        services.AddTransient<RecipeStepRepository>();

        services.AddSingleton<IDialogService, DialogService>();

        return services.BuildServiceProvider();
    } 
}