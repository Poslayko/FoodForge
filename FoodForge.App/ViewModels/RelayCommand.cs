using System.Windows.Input;

namespace FoodForge.App.ViewModels;

public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action execute)
    {
        _execute = _ => execute();
    }
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
        _execute = _ => execute();
        _canExecute = _ => canExecute();
    }
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parametr)
    {
        return _canExecute?.Invoke(parametr) ?? true;
    }

    public void Execute(object? parametr)
    {
        _execute(parametr);
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}