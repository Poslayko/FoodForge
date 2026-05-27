using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
public sealed class DishServices
{
    public static FullEditingDish Update(FullEditingDish dish)
    {
        using var db = FoodForgeDbContextProvider.Create();

        var dishId = dish.Id;

        if (!db.Dishes.Any(x => x.Id == dishId))
        {
            throw new InvalidOperationException($"Dish with id {dishId} was not found.");
        }

        UpdateDishIngredients(db, dishId, dish);
        UpdateDishRecipeSteps(db, dishId, dish);

        dish.UpdatedAt = DateTime.UtcNow;

        db.Update(ConvertToDish(dish));
        db.SaveChanges();

        return dish;
    }

    private static void UpdateDishRecipeSteps(FoodForgeDbContext db, int dishId,
        FullEditingDish dish)
    {
        var dbSteps = db.RecipeSteps
            .Where(x => x.DishId == dishId)
            .ToList();

        var existingStepIds = dish.RecipeSteps
            .Where(x => x.Id != 0)
            .Select(x => x.Id)
            .ToHashSet();

        var stepsToDelete = dbSteps
            .Where(x => !existingStepIds.Contains(x.Id))
            .ToList();

        db.RecipeSteps.RemoveRange(stepsToDelete);

        foreach (var step in dish.RecipeSteps)
        {
            step.DishId = dishId;

            if (step.Id == 0)
            {
                db.RecipeSteps.Add(step);
                continue;
            }

            var dbStep = dbSteps.First(x => x.Id == step.Id);

            dbStep.Order = step.Order;
            dbStep.Description = step.Description;
            dbStep.TimeMinutes = step.TimeMinutes;
            dbStep.Comment = step.Comment;
        }
    }

    private static void UpdateDishIngredients(FoodForgeDbContext db, int dishId, 
        FullEditingDish dish)
    {
        List<FullDishIngredient> dishIngredients = dish.Ingredients;
        List<Ingredient> ingredients;

        if (dishIngredients != null)
        {
            List<string> dishIngredientsName = dishIngredients.Select(x => x.Name).ToList();
            ingredients = IngredientRepository.CreateIfNonExist(db, dishIngredientsName);

            List<DishIngredient> updatedDishIngredients = new();

            for (int x = 0; x < dishIngredients.Count; x++)
            {
                FullDishIngredient dishIngredient = dish.Ingredients[x];
                dishIngredient.IngredientId = ingredients
                    .First(ingredient => ingredient.Name == dishIngredient.Name)
                    .Id;

                var updatedIngredient = new DishIngredient()
                {
                    DishId = dishIngredient.DishId,
                    IngredientId = dishIngredient.IngredientId,
                    Order = dishIngredient.Order,
                    Quantity = dishIngredient.Quantity,
                    MeasurementUnit = dishIngredient.MeasurementUnit,
                    Comment = dishIngredient.Comment
                };

                updatedDishIngredients.Add(updatedIngredient);
            }

            DishIngredientRepository.Delete(db, dishId);
            DishIngredientRepository.Create(db, updatedDishIngredients);
        }
    }

    public static Dish ConvertToDish(FullEditingDish dish)
    {
        return new Dish()
        {
            Id = dish.Id,
            ProfileId = dish.ProfileId,
            Name = dish.Name!,
            Description = dish.Description!,
            TasteRating = dish.TasteRating,
            SpentTimeMinutes = dish.SpentTimeMinutes,
            CreatedAt = dish.CreatedAt,
            UpdatedAt = dish.UpdatedAt
        };
    }

    public static FullEditingDish ConvertToFullEditingDish(Dish dbDish)
    {
        FullEditingDish dish = new()
        {
            Id = dbDish.Id,
            ProfileId = dbDish.ProfileId,
            Name = dbDish.Name,
            Description = dbDish.Description,
            TasteRating = dbDish.TasteRating,
            SpentTimeMinutes = dbDish.SpentTimeMinutes,
            CreatedAt = dbDish.CreatedAt,
            UpdatedAt = dbDish.UpdatedAt,
            Ingredients = ConvertDishIngredientsToEditable(dbDish.DishIngredients) 
        };

        return dish;
    }

    public static List<FullEditingDish> ConvertToFullEditingDishes(List<Dish> dbDishes)
    {
        List<FullEditingDish> dishes = new();
        foreach(var dish in dbDishes)
        {
            dishes.Add(new FullEditingDish()
            {
                Id = dish.Id,
                ProfileId = dish.ProfileId,
                Name = dish.Name,
                Description = dish.Description,
                TasteRating = dish.TasteRating,
                SpentTimeMinutes = dish.SpentTimeMinutes,
                CreatedAt = dish.CreatedAt,
                UpdatedAt = dish.UpdatedAt,
                Ingredients = ConvertDishIngredientsToEditable(dish.DishIngredients)
            });
        }

        return dishes;
    }

    public static List<FullDishIngredient> ConvertDishIngredientsToEditable(
        List<DishIngredient> dbIngredients)
    {
        List<FullDishIngredient> ingredients = new();
        foreach(var ingredient in dbIngredients)
        {
            ingredients.Add(new FullDishIngredient()
            {
                Id = ingredient.Id,
                DishId = ingredient.DishId,
                IngredientId = ingredient.IngredientId,
                Order = ingredient.Order,
                Quantity = ingredient.Quantity,
                MeasurementUnit = ingredient.MeasurementUnit,
                Comment = ingredient.Comment,
                Name = ingredient.Ingredient.Name
            });
        }

        return ingredients;
    }
}