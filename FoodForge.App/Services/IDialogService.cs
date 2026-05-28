using Avalonia.Controls;

namespace FoodForge.App.Services;

public interface IDialogService
{
    void SetOwner(Window owner);
    Task<bool> ConfirmAsync(string title, string message);
}