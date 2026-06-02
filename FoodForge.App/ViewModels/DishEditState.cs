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

    private ObservableCollection<RecipeStep> _editingRecipeSteps = new();
    public ObservableCollection<RecipeStep> EditingRecipeSteps
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

public DishEditState()
    {        
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

        int tasteRating = EditingDish.TasteRating;
        int spentTimeMinutes = EditingDish.SpentTimeMinutes;

        if (string.IsNullOrWhiteSpace(EditingDish.Name)
            || tasteRating < 0 || tasteRating > 10
            || spentTimeMinutes < 0 || spentTimeMinutes > 2000)
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
            .Select((x, index) => new FullDishIngredient
            {
                Name = x.Name.Trim(),
                Quantity = x.Quantity.Trim(),
                Order = index + 1
            })
            .ToList();
        
        return ingredientsForSave;
    }

    private List<RecipeStep> BuildRecipeStepsForSave()
    {
        var recipeStepsForSave = EditingRecipeSteps
            .Where(x => !string.IsNullOrWhiteSpace(x.Description) ||
                x.TimeMinutes < 0)
            .Select((x, index) => new RecipeStep
            {
                Description = x.Description.Trim(),
                Comment = x.Comment?.Trim(),
                Order = index + 1
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
    }

    private void PushUpIngredient(object? parameter)
    {
        if (parameter is not FullDishIngredient ingredient)
        {
            return;
        }

        int ingredientIndex = EditingIngredients.IndexOf(ingredient);
        int previousIndex = ingredientIndex - 1;

        if (ingredientIndex <= 0)
        {
            return;
        }

        EditingIngredients.Move(ingredientIndex, previousIndex);

        RefreshIngredientOrders();
    }

    private void PushDownIngredient(object? parameter)
    {
        if (parameter is not FullDishIngredient ingredient)
        {
            return;
        }

        int ingredientIndex = EditingIngredients.IndexOf(ingredient);
        int nextIndex = ingredientIndex + 1;

        if (ingredientIndex >= EditingIngredients.Count - 1)
        {
            return;
        }

        EditingIngredients.Move(ingredientIndex, nextIndex);

        RefreshIngredientOrders();
    }
    private void RefreshIngredientOrders()
    {
        for (int i = 0; i < EditingIngredients.Count; i++)
        {
            EditingIngredients[i].Order = i + 1;
        }

        EditingIngredients = new ObservableCollection<FullDishIngredient>(EditingIngredients);
    }

    private void AddRecipeStep()
    {
        if (EditingDish is null)
        {
            return;
        }

        int order = EditingRecipeSteps.Count + 1;
        EditingRecipeSteps.Add(new RecipeStep()
        {
            Order = order,
            DishId = EditingDish.Id
        });
    }

    private void DeleteRecipeStep(object? parameter)
    {
        if (parameter is not RecipeStep step)
        {
            return;
        }

        EditingRecipeSteps.Remove(step);
    }

    private void PushUpRecipeStep(object? parameter)
    {
        if (parameter is not RecipeStep step)
        {
            return;
        }

        int stepIndex = EditingRecipeSteps.IndexOf(step);
        int previousIndex = stepIndex - 1;

        if (stepIndex <= 0)
        {
            return;
        }

        EditingRecipeSteps.Move(stepIndex, previousIndex);

        RefreshRecipeStepOrders();
    }

    private void PushDownRecipeStep(object? parameter)
    {
        if (parameter is not RecipeStep step)
        {
            return;
        }

        int stepIndex = EditingRecipeSteps.IndexOf(step);
        int nextIndex = stepIndex + 1;

        if (stepIndex >= EditingRecipeSteps.Count - 1)
        {
            return;
        }

        EditingRecipeSteps.Move(stepIndex, nextIndex);

        RefreshRecipeStepOrders();
    }

    private void RefreshRecipeStepOrders()
    {
        for (int i = 0; i < EditingRecipeSteps.Count; i++)
        {
            EditingRecipeSteps[i].Order = i + 1;
        }

        EditingRecipeSteps = new ObservableCollection<RecipeStep>(EditingRecipeSteps);
    }

}