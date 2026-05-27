using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public sealed class DishIngredientRepository
{
    public List<FullDishIngredient> GetAllByDishId(int DishId)
    {
        using var db = FoodForgeDbContextProvider.Create();

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

    public static void Delete(FoodForgeDbContext db, int dishId)
    {
        var dishIngredients = db.DishIngredients.Where(x => x.DishId == dishId).ToList();
        db.DishIngredients.RemoveRange(dishIngredients);
    }

    public static void Create(FoodForgeDbContext db, List<DishIngredient> dishIngredients)
    {
        db.DishIngredients.AddRange(dishIngredients);
    }
}