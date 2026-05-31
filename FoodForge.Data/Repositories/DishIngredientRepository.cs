using Microsoft.EntityFrameworkCore;

public sealed class DishIngredientRepository
{
    private readonly IDbContextFactory<FoodForgeDbContext> _dbContextFactory;

    public DishIngredientRepository(IDbContextFactory<FoodForgeDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    public List<FullDishIngredient> GetAllByDishId(int DishId)
    {
        using var db = _dbContextFactory.CreateDbContext(); 

        return db.DishIngredients
            .Where(x => x.DishId == DishId)
            .OrderBy(x => x.Order)
            .Select(x => new FullDishIngredient
            {
                Id = x.Id,
                DishId = x.DishId,
                IngredientId = x.IngredientId,
                Name = x.Ingredient.Name,
                Order = x.Order,
                Quantity = x.Quantity,
                MeasurementUnit = x.MeasurementUnit,
                Comment = x.Comment
            })
            .ToList();
    }

    public void Delete(FoodForgeDbContext db, int dishId)
    {
        var dishIngredients = db.DishIngredients.Where(x => x.DishId == dishId).ToList();
        db.DishIngredients.RemoveRange(dishIngredients);
    }

    public void Create(FoodForgeDbContext db, List<DishIngredient> dishIngredients)
    {
        db.DishIngredients.AddRange(dishIngredients);
    }
}