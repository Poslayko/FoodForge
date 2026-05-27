using Microsoft.EntityFrameworkCore;

public sealed class RecipeStepRepository
{
    public List<RecipeStep> GetAllByDishId(int dishId)
    {
        using var db = FoodForgeDbContextProvider.Create();

        return db.RecipeSteps
            .Where(x => x.DishId == dishId)
            .OrderBy(x => x.Order)
            .ToList();
    }
}