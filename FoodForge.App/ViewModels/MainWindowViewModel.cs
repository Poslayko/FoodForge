using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using FoodForge.App.Services;

namespace FoodForge.App.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ProfileRepository _profileRepository;
    private readonly DishService _dishService;
    private readonly DishRepository _dishRepository;
    private readonly DishIngredientRepository _dishIngredientRepository;
    private readonly RecipeStepRepository _recipeStepRepository;
    private readonly IDialogService _dialogService;

    public ObservableCollection<Profile> Profiles { get; private set; } = new();
    private ObservableCollection<FullEditingDish> _dishes = new();
    public ObservableCollection<FullEditingDish> Dishes
    {
        get => _dishes;
        set
        {
            if (_dishes == value)
                return;

            _dishes = value;
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
    
    
    public Profile? SelectedProfile { get; set; }
    private FullEditingDish? _selectedDish;
    public FullEditingDish? SelectedDish
    {
        get => _selectedDish;
        set
        {
            if (_selectedDish == value)
            {
                return;
            }

            _selectedDish = value;

            OnPropertyChanged();

            LoadSelectedDishDetails();
        }
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

    public event PropertyChangedEventHandler? PropertyChanged;
    public bool HasNoDishes => Dishes.Count == 0;
    public bool HasDishes => Dishes.Count > 0;
    public bool IsInStandardDisplayDishMode { get; set; } = true;
    public bool IsInEditDisplayDishMode => !IsInStandardDisplayDishMode;
    public ICommand CreateDishCommand { get; }
    public ICommand DeleteDishCommand { get; }
    public ICommand ChangeDisplayModeOnEditCommand { get; }
    public ICommand SaveEditingDishCommand { get; }
    public ICommand CancelEditingCommand { get; }
    public ICommand AddIngredientCommand { get; }
    public ICommand DeleteIngredientCommand { get; }
    public ICommand PushUpIngredientCommand { get; }
    public ICommand PushDownIngredientCommand { get; }
    public ICommand AddRecipeStepCommand { get; }
    public ICommand DeleteRecipeStepCommand { get; }
    public ICommand PushUpRecipeStepCommand { get; }
    public ICommand PushDownRecipeStepCommand { get; }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainWindowViewModel(IDialogService dialogService, ProfileRepository profileRepository,
        DishRepository dishRepository, DishService dishService, 
        DishIngredientRepository dishIngredientRepository, 
        RecipeStepRepository recipeStepRepository)
    {
        _profileRepository = profileRepository;
        _dishRepository = dishRepository;        
        _dishService = dishService;
        _dishIngredientRepository = dishIngredientRepository;
        _recipeStepRepository = recipeStepRepository;
        _dialogService = dialogService;

        CreateDishCommand = new RelayCommand(CreateDish);
        DeleteDishCommand = new RelayCommand(DeleteDish);
        ChangeDisplayModeOnEditCommand = new RelayCommand(ChangeDisplayModeOnEdit);
        SaveEditingDishCommand = new RelayCommand(SaveEditingDish);
        CancelEditingCommand = new RelayCommand(CancelEditing);
        AddIngredientCommand = new RelayCommand(AddIngredient);                               
        DeleteIngredientCommand = new RelayCommand(DeleteIngredient);
        PushUpIngredientCommand = new RelayCommand(PushUpIngredient);
        PushDownIngredientCommand = new RelayCommand(PushDownIngredient);
        AddRecipeStepCommand = new RelayCommand(AddRecipeStep);
        DeleteRecipeStepCommand = new RelayCommand(DeleteRecipeStep);
        PushUpRecipeStepCommand = new RelayCommand(PushUpRecipeStep);
        PushDownRecipeStepCommand = new RelayCommand(PushDownRecipeStep);

        Profiles = new ObservableCollection<Profile>(_profileRepository.GetAll());

        var firstProfile = Profiles.FirstOrDefault();

        if (firstProfile is null)
        {
            Dishes = new ObservableCollection<FullEditingDish>();
            return;
        }

        SelectedProfile = firstProfile;

        List<Dish> dbDishes = _dishRepository.GetAllById(firstProfile.Id);

        Dishes = new ObservableCollection<FullEditingDish>(
            DishService.ConvertToFullEditingDishes(dbDishes));

        SelectedDish = Dishes.FirstOrDefault();
    }

    private void LoadSelectedDishDetails()
    {
        if (SelectedDish is null)
        {
            return;
        }

        int dishId = SelectedDish.Id;

        SelectedDish.Ingredients = _dishIngredientRepository.GetAllByDishId(dishId);
        SelectedDish.RecipeSteps = _recipeStepRepository.GetAllByDishId(dishId);


        OnPropertyChanged(nameof(SelectedDish));
    }

    private void CreateDish()
    {
        if (SelectedProfile is null)
        {
            return;
        }

        var createdDish = DishService.ConvertToFullEditingDish(
            _dishRepository.Create(SelectedProfile.Id));

        Dishes.Insert(0, createdDish);
        SelectedDish = createdDish;

        NotifyDishesChanged();
    }

    private void NotifyDishesChanged()
    {
        OnPropertyChanged(nameof(HasDishes));
        OnPropertyChanged(nameof(HasNoDishes));
    }

    private async void DeleteDish(object? parameter)
    {
        if (SelectedDish is null)
        {
            return;
        }

        bool confirmed = await _dialogService.ConfirmAsync(
            "Delete dish?",
            $"Are you sure you want to delete \"{SelectedDish.Name}\"?");

        if (!confirmed)
        {
            return;
        }

        var dishToDelete = SelectedDish;

        _dishRepository.Delete(dishToDelete.Id);
        Dishes.Remove(dishToDelete);
        
        EditingDish = null;
        EditingIngredients.Clear();
        EditingRecipeSteps.Clear();
        IsInStandardDisplayDishMode = true;
        
        if (Dishes.Count() != 0)
        {
            SelectedDish = Dishes[0];
        }
        else
        {
            SelectedDish = null;
        }

        ModeNotifier();
        NotifyDishesChanged();
    }

    private void SaveEditingDish()
    {
        if (EditingDish is null || SelectedDish is null)
        {
            return;
        }

        int tasteRating = EditingDish.TasteRating;
        int spentTimeMinutes = EditingDish.SpentTimeMinutes;

        if (string.IsNullOrWhiteSpace(EditingDish.Name)
            || tasteRating < 0 || tasteRating > 10
            || spentTimeMinutes < 0 || spentTimeMinutes > 2000)
        {
            return;
        }

        AddEditingIngredientsAndDeleteEmpty();
        AddEditingRecipeStepsAndDeleteEmpty();

        var savedDish = _dishService.Update(EditingDish);

        var index = Dishes.IndexOf(SelectedDish);

        if (index >= 0)
        {
            Dishes[index] = savedDish;
        }

        SelectedDish = savedDish;
        EditingDish = null;

        ChangeEditModeOnDisplay();
    }

    private void AddEditingRecipeStepsAndDeleteEmpty()
    {
        if (EditingDish is null)
        {
            return;
        }

        List<RecipeStep> recipeSteps = EditingRecipeSteps.ToList();

        int Order = 1;

        for (int x = 0; x < recipeSteps.Count; )
        {
            if (string.IsNullOrWhiteSpace(recipeSteps[x].Description) ||
                recipeSteps[x].TimeMinutes == 0)
            {
                recipeSteps.Remove(recipeSteps[x]);
                continue;
            }

            recipeSteps[x].Order = Order;
            Order++;
            x++;
        }

        EditingDish.RecipeSteps = recipeSteps;
    }

    private void AddEditingIngredientsAndDeleteEmpty()
    {
        if (EditingDish is null)
        {
            return;
        }

        List<FullDishIngredient> dishIngredients = EditingIngredients.ToList();

        int Order = 1;

        for (int x = 0; x < dishIngredients.Count; )
        {
            if (string.IsNullOrWhiteSpace(dishIngredients[x].Name) || 
                string.IsNullOrWhiteSpace(dishIngredients[x].Quantity))
            {
                dishIngredients.Remove(dishIngredients[x]);
                continue;
            }

            dishIngredients[x].Order = Order;
            Order++;
            x++;
        }

        EditingDish.Ingredients = dishIngredients;
    }
    private void ChangeDisplayModeOnEdit()
    {
        PrepareEditingDish();
        IsInStandardDisplayDishMode = false;
        ModeNotifier();
    }

    private void PrepareEditingDish()
    {
        if (SelectedDish is null)
        {
            return;
        }

        EditingIngredients = new ObservableCollection<FullDishIngredient>(
            SelectedDish.Ingredients.Select(x => new FullDishIngredient
            {
                Id = x.Id,
                DishId = x.DishId,
                IngredientId = x.IngredientId,
                Name = x.Name,
                Order = x.Order,
                Quantity = x.Quantity,
                MeasurementUnit = x.MeasurementUnit,
                Comment = x.Comment
            })
        );

        EditingRecipeSteps = new ObservableCollection<RecipeStep>(
            SelectedDish.RecipeSteps.Select(x => new RecipeStep
            {
                Id = x.Id,
                DishId = x.DishId,
                Order = x.Order,
                Description = x.Description,
                TimeMinutes = x.TimeMinutes,
                Comment = x.Comment
            })
        );

        EditingDish = new FullEditingDish
        {
            Id = SelectedDish.Id,
            ProfileId = SelectedDish.ProfileId,
            Name = SelectedDish.Name,
            Description = SelectedDish.Description,
            TasteRating = SelectedDish.TasteRating,
            SpentTimeMinutes = SelectedDish.SpentTimeMinutes,
            CreatedAt = SelectedDish.CreatedAt,
            UpdatedAt = SelectedDish.UpdatedAt
        };
    }

    private void ChangeEditModeOnDisplay()
    {
        IsInStandardDisplayDishMode = true;
        ModeNotifier();
    }

    private void ModeNotifier()
    {
        OnPropertyChanged(nameof(IsInEditDisplayDishMode));
        OnPropertyChanged(nameof(IsInStandardDisplayDishMode));
        OnPropertyChanged(nameof(EditingIngredients));
        OnPropertyChanged(nameof(EditingRecipeSteps));
    }

    private void CancelEditing()
    {
        ChangeEditModeOnDisplay();
        EditingDish = null;
    }

    private void AddIngredient()
    {
        if (EditingDish is null)
        {
            return;
        }

        int Order = EditingIngredients.Count() + 1;
        EditingIngredients.Add(new FullDishIngredient()
        {
            Order = Order,
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

        if (ingredientIndex >= EditingIngredients.Count() - 1)
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

        int Order = EditingRecipeSteps.Count() + 1;
        EditingRecipeSteps.Add(new RecipeStep()
        {
            Order = Order,
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

        if (stepIndex >= EditingRecipeSteps.Count() - 1)
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