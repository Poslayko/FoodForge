using Microsoft.EntityFrameworkCore;

public sealed class RecipeStepRepository
{
    private readonly IDbContextFactory<FoodForgeDbContext> _dbContextFactory;
    
    public RecipeStepRepository(IDbContextFactory<FoodForgeDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    public List<RecipeStep> GetAllByDishId(int dishId)
    {
        using var db = _dbContextFactory.CreateDbContext();

        return db.RecipeSteps
            .Where(x => x.DishId == dishId)
            .OrderBy(x => x.Order)
            .ToList();
    }
}