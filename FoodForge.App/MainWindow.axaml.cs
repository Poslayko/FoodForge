using Avalonia.Controls;
using FoodForge.App.Services;
using FoodForge.App.ViewModels;

namespace FoodForge.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    public MainWindow(MainWindowViewModel viewModel, IDialogService dialogService)
        : this()
    {
        dialogService.SetOwner(this);

        DataContext = viewModel;
    }
}