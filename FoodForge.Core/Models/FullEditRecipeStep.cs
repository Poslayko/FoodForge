using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class FullEditRecipeStep : INotifyPropertyChanged, IOrderedItem
{
    private string? _timeMinutesError;
    public int Id { get; set; }
    public int DishId { get; set; }
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TimeMinutes { get; set; }
    public string? Comment { get; set; }
    public string? TimeMinutesError
    {
        get => _timeMinutesError;
        set
        {
            if (_timeMinutesError == value)
                return;

            _timeMinutesError = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasTimeMinutesError));
        }
    }

    public bool HasTimeMinutesError => !string.IsNullOrWhiteSpace(TimeMinutesError);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}