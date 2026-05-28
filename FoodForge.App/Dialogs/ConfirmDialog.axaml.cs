using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FoodForge.App.Dialogs;

public partial class ConfirmDialog : Window
{
    public string Message { get; set; } = string.Empty;

    public ConfirmDialog()
    {
        InitializeComponent();

        DataContext = this;
    }
    public ConfirmDialog(string title, string message) 
    {
        InitializeComponent();

        Title = title;
        Message = message;
        DataContext = this;
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}