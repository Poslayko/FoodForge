public sealed class DishEditValidator 
{
    public void Validate(DishEditState dishState)
    {
        dishState.ClearDataForValidation();

        if (dishState.EditingDish is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(dishState.EditingDish.Name))
        {
            dishState.SetNameError("Dish name is required.");
        }

        if (dishState.EditingDish.TasteRating < 0 || dishState.EditingDish.TasteRating > 10)
        {
            dishState.SetTasteRatingError("Taste rating must be between 0 and 10.");
        }       

        if (dishState.EditingDish.SpentTimeMinutes < 0 || 
            dishState.EditingDish.SpentTimeMinutes > 2000)
        {
            dishState.SetSpentTimeMinutesError("Spent time minutes can be between 0 and 2000.");
        }

        foreach (var step in dishState.EditingRecipeSteps)
        {
            if (step.TimeMinutes < 0)
            {
                step.TimeMinutesError = $"Time minutes must be 0 or greater";
                dishState.SetHasRecipeStepTimeMinutesErrors(true);
            }
        }
    }
}