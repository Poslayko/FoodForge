using Avalonia.Controls;
using FoodForge.App.Dialogs;

namespace FoodForge.App.Services;

public sealed class DialogService : IDialogService
{
    private Window? _owner;

    public void SetOwner(Window owner)
    {
        _owner = owner;
    }

    public async Task<bool> ConfirmAsync(string title, string message)
    {
        if (_owner is null)
        {
            throw new InvalidOperationException("Dialog owner was not set.");
        }

        var dialog = new ConfirmDialog(title, message);

        return await dialog.ShowDialog<bool>(_owner);
    }
}