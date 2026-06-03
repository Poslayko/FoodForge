using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using FoodForge.App.Services;

namespace FoodForge.App.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ProfileRepository _profileRepository;
    private readonly DishRepository _dishRepository;
    private readonly DishIngredientRepository _dishIngredientRepository;
    private readonly RecipeStepRepository _recipeStepRepository;
    private readonly DishService _dishService;
    private readonly ReorderService _reorderService;
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

    private DishEditState? _editState;
    public DishEditState? EditState
    {
        get => _editState;
        set
        {
            if (_editState == value)
            {
                return;
            }

            _editState = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsInEditMode));
            OnPropertyChanged(nameof(IsInViewMode));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public bool HasNoDishes => Dishes.Count == 0;
    public bool HasDishes => Dishes.Count > 0;
    public bool IsInEditMode => EditState is not null;
    public bool IsInViewMode => !IsInEditMode;

    public ICommand CreateDishCommand { get; }
    public ICommand DeleteDishCommand { get; }
    public ICommand ChangeViewModeOnEditCommand { get; }
    public ICommand CancelEditingCommand { get; }
    public ICommand SaveEditingDishCommand { get; }        

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainWindowViewModel(IDialogService dialogService, 
        ProfileRepository profileRepository, DishRepository dishRepository, 
        DishIngredientRepository dishIngredientRepository, 
        DishService dishService, RecipeStepRepository recipeStepRepository,
        ReorderService reorderService)
    {
        _profileRepository = profileRepository;
        _dishRepository = dishRepository;
        _dishIngredientRepository = dishIngredientRepository;
        _recipeStepRepository = recipeStepRepository;
        _dishService = dishService;
        _reorderService = reorderService;
        _dialogService = dialogService;

        CreateDishCommand = new RelayCommand(CreateDish);
        DeleteDishCommand = new RelayCommand(DeleteDish);
        ChangeViewModeOnEditCommand = new RelayCommand(ChangeViewModeOnEdit);
        CancelEditingCommand = new RelayCommand(CancelEditing);
        SaveEditingDishCommand = new RelayCommand(SaveEditingDish);

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

    private void SaveEditingDish()
    {
        if (SelectedDish is null || EditState is null)
        {
            return;
        }

        var editingDish = EditState.BuildDishForSave();

        if (editingDish is null)
        {
            return;
        }

        var savedDish = _dishService.Update(editingDish);

        var index = Dishes.IndexOf(SelectedDish);

        if (index >= 0)
        {
            Dishes[index] = savedDish;
        }

        SelectedDish = savedDish;
        EditState = null;
    }

    private void LoadSelectedDishDetails()
    {
        if (SelectedDish is null)
        {
            EditState = null;
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

        EditState = null;

        if (Dishes.Count != 0)
        {
            SelectedDish = Dishes[0];
        }
        else
        {
            SelectedDish = null;
        }

        NotifyDishesChanged();
    }

    private void ChangeViewModeOnEdit()
    {
        PrepareEditingDish();
    }

    public void PrepareEditingDish()
    {
        if (SelectedDish is null)
        {
            return;
        }

        var editState = new DishEditState(_reorderService);

        editState.EditingIngredients = new ObservableCollection<FullDishIngredient>(
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

        editState.EditingRecipeSteps = new ObservableCollection<RecipeStep>(
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

        editState.EditingDish = new FullEditingDish
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

        EditState = editState;
    }

    private void CancelEditing()
    {
        EditState = null;
    }

}