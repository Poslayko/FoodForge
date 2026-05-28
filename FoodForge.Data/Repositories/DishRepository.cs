using Microsoft.EntityFrameworkCore;

public sealed class DishRepository
{
    public List<Dish> GetAll()
    {
        using var db = FoodForgeDbContextProvider.Create();

        return db.Dishes
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

        public List<Dish> GetAllById(int profileId)
        {
        using var db = FoodForgeDbContextProvider.Create();

        return db.Dishes
            .Include(x => x.DishIngredients)
                .ThenInclude(x => x.Ingredient)
            .Where(x => x.ProfileId == profileId)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public Dish Create(int profileId)
    {
        using var db = FoodForgeDbContextProvider.Create();

        if (!db.Profile.Any(x => x.Id == profileId))
        {
            throw new InvalidOperationException($"Profile with id {profileId} was not found.");
        }

        var now = DateTime.UtcNow;
        int randomDishNumber = new Random().Next(1, 100);

        var templateDish = new Dish()
        {
            ProfileId = profileId,
            Name = $"New Dish {randomDishNumber}",
            Description = "",
            TasteRating = 0,
            SpentTimeMinutes = 0,
            CreatedAt = now,
            UpdatedAt = now 
        };

        db.Dishes.Add(templateDish);
        db.SaveChanges();

        return templateDish;
    }

    public void Delete(int dishId)
    {
        using var db = FoodForgeDbContextProvider.Create();

        var dish = db.Dishes.FirstOrDefault(x => x.Id == dishId);

        if (dish is null)
        {
            return;
        }

        db.Dishes.Remove(dish);
        db.SaveChanges();
    }
}