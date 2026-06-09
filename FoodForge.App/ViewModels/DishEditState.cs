using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FoodForge.App.ViewModels;

public sealed class DishEditState : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly ReorderService _reorderService;

    private FullEditingDish? _editingDish;
    public FullEditingDish? EditingDish
    {
        get => _editingDish;
        set
        {
            if (_editingDish == value)
            {
                return;
            }

            _editingDish = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<FullDishIngredient> _editingIngredients = new();
    public ObservableCollection<FullDishIngredient> EditingIngredients
    {
        get => _editingIngredients;
        set
        {
            if (_editingIngredients == value)
                return;

            _editingIngredients = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<FullEditRecipeStep> _editingRecipeSteps = new();
    public ObservableCollection<FullEditRecipeStep> EditingRecipeSteps
    {
        get => _editingRecipeSteps;
        set
        {
            if (_editingRecipeSteps == value)
                return;

            _editingRecipeSteps = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddIngredientCommand { get; }
    public ICommand DeleteIngredientCommand { get; }
    public ICommand PushUpIngredientCommand { get; }
    public ICommand PushDownIngredientCommand { get; }
    public ICommand AddRecipeStepCommand { get; }
    public ICommand DeleteRecipeStepCommand { get; }
    public ICommand PushUpRecipeStepCommand { get; }
    public ICommand PushDownRecipeStepCommand { get; }

    public string? NameError { get; private set; } 
    public string? TasteRatingError { get; private set; }
    public string? SpentTimeMinutesError { get; private set; }

    public bool HasDishNameError => NameError is not null;
    public bool HasTasteRatingError => TasteRatingError is not null;
    public bool HasSpentTimeMinutesError => SpentTimeMinutesError is not null;
    public bool HasRecipeStepTimeMinutesErrors { get; private set; } = false;

    public bool HasValidationErrors =>
        HasDishNameError ||
        HasTasteRatingError ||
        HasSpentTimeMinutesError ||
        HasRecipeStepTimeMinutesErrors; 

    public DishEditState(ReorderService reorderService)
    {        
        _reorderService = reorderService;
        AddIngredientCommand = new RelayCommand(AddIngredient);
        DeleteIngredientCommand = new RelayCommand(DeleteIngredient);
        PushUpIngredientCommand = new RelayCommand(PushUpIngredient);
        PushDownIngredientCommand = new RelayCommand(PushDownIngredient);
        AddRecipeStepCommand = new RelayCommand(AddRecipeStep);
        DeleteRecipeStepCommand = new RelayCommand(DeleteRecipeStep);
        PushUpRecipeStepCommand = new RelayCommand(PushUpRecipeStep);
        PushDownRecipeStepCommand = new RelayCommand(PushDownRecipeStep);
    }

    public FullEditingDish? BuildDishForSave()
    {
        if (EditingDish is null)
        {
            return null;
        }

        EditingDish.Ingredients = BuildIngredientsForSave();
        EditingDish.RecipeSteps = BuildRecipeStepsForSave();

        return EditingDish;
    }

    private List<FullDishIngredient> BuildIngredientsForSave()
    {
        var ingredientsForSave = EditingIngredients
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select((x, index) => new FullDishIngredient()
            {
                Name = x.Name.Trim(),
                Quantity = x.Quantity.Trim(),
                Order = index + 1,
                MeasurementUnit = x.MeasurementUnit?.Trim(),
                Comment = x.Comment?.Trim(),
                DishId = x.DishId
            })
            .ToList();
        
        return ingredientsForSave;
    }

    private List<FullEditRecipeStep> BuildRecipeStepsForSave()
    {
        var recipeStepsForSave = EditingRecipeSteps
            .Where(x => !string.IsNullOrWhiteSpace(x.Description))
            .Select((x, index) => new FullEditRecipeStep 
            {
                Description = x.Description.Trim(),
                Comment = x.Comment?.Trim(),
                Order = index + 1,
                DishId = x.DishId,
                Id = x.Id,
                TimeMinutes = x.TimeMinutes
            })
            .ToList();

        return recipeStepsForSave;
    }

    private void AddIngredient()
    {
        if (EditingDish is null)
        {
            return;
        }

        int order = EditingIngredients.Count + 1;
        EditingIngredients.Add(new FullDishIngredient()
        {
            Order = order,
            DishId = EditingDish.Id
        });
    }

    private void DeleteIngredient(object? parameter)
    {
        if (parameter is not FullDishIngredient ingredient)
        {
            return;
        }

        EditingIngredients.Remove(ingredient);

        _reorderService.RefreshOrders(EditingIngredients);

        EditingIngredients = new ObservableCollection<FullDishIngredient>(EditingIngredients);
    }

    private void PushUpIngredient(object? parameter)
    {
        if (parameter is not FullDishIngredient ingredient)
        {
            return;
        }

        _reorderService.MoveUp(EditingIngredients, ingredient);

        EditingIngredients = new ObservableCollection<FullDishIngredient>(EditingIngredients);
    }

    private void PushDownIngredient(object? parameter)
    {
        if (parameter is not FullDishIngredient ingredient)
        {
            return;
        }

        _reorderService.MoveDown(EditingIngredients, ingredient);

        EditingIngredients = new ObservableCollection<FullDishIngredient>(EditingIngredients);
    }

    private void AddRecipeStep()
    {
        if (EditingDish is null)
        {
            return;
        }

        int order = EditingRecipeSteps.Count + 1;
        EditingRecipeSteps.Add(new FullEditRecipeStep()
        {
            Order = order,
            DishId = EditingDish.Id
        });
    }

    private void DeleteRecipeStep(object? parameter)
    {
        if (parameter is not FullEditRecipeStep step)
        {
            return;
        }

        EditingRecipeSteps.Remove(step);

        _reorderService.RefreshOrders(EditingRecipeSteps);

        EditingRecipeSteps = new ObservableCollection<FullEditRecipeStep>(EditingRecipeSteps);
    }

    private void PushUpRecipeStep(object? parameter)
    {
        if (parameter is not FullEditRecipeStep step)
        {
            return;
        }

        _reorderService.MoveUp(EditingRecipeSteps, step);

        EditingRecipeSteps = new ObservableCollection<FullEditRecipeStep>(EditingRecipeSteps);
    }

    private void PushDownRecipeStep(object? parameter)
    {
        if (parameter is not FullEditRecipeStep step)
        {
            return;
        }

        _reorderService.MoveDown(EditingRecipeSteps, step);

        EditingRecipeSteps = new ObservableCollection<FullEditRecipeStep>(EditingRecipeSteps);
    }

    public void ClearDataForValidation()
    {
        NameError = null;
        SpentTimeMinutesError = null;
        TasteRatingError = null;
        foreach (var step in EditingRecipeSteps) step.TimeMinutesError = null;
        HasRecipeStepTimeMinutesErrors = false;
        OnPropertyChanged(nameof(NameError));
        OnPropertyChanged(nameof(HasDishNameError));
        OnPropertyChanged(nameof(TasteRatingError));
        OnPropertyChanged(nameof(HasTasteRatingError));
        OnPropertyChanged(nameof(SpentTimeMinutesError));
        OnPropertyChanged(nameof(HasSpentTimeMinutesError));
        OnPropertyChanged(nameof(HasRecipeStepTimeMinutesErrors));
        OnPropertyChanged(nameof(HasValidationErrors));
    }

    public void SetNameError(string error)
    {
        NameError = error;
        OnPropertyChanged(nameof(NameError));
        OnPropertyChanged(nameof(HasDishNameError));
        OnPropertyChanged(nameof(HasValidationErrors));
    }

    public void SetTasteRatingError(string error)
    {
        TasteRatingError = error;
        OnPropertyChanged(nameof(TasteRatingError));
        OnPropertyChanged(nameof(HasTasteRatingError));
        OnPropertyChanged(nameof(HasValidationErrors));
    }

    public void SetSpentTimeMinutesError(string error)
    {
        SpentTimeMinutesError = error;
        OnPropertyChanged(nameof(SpentTimeMinutesError));
        OnPropertyChanged(nameof(HasSpentTimeMinutesError));
        OnPropertyChanged(nameof(HasValidationErrors));
    }

    public void SetHasRecipeStepTimeMinutesErrors(bool b)
    {
        HasRecipeStepTimeMinutesErrors = b;
        OnPropertyChanged(nameof(HasRecipeStepTimeMinutesErrors));
        OnPropertyChanged(nameof(HasValidationErrors));
    }
}