using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
public sealed class IngredientRepository
{
    public static List<Ingredient> CreateIfNonExist(FoodForgeDbContext db, 
        List<string> names)
    {
        List<string> existingIngredients = db.Ingredients
            .Where(x => names.Contains(x.Name))
            .Select(x => x.Name)
            .ToList();

        foreach (var name in names)
        {
            if (!existingIngredients.Contains(name))
            {
                var newIngredient = new Ingredient()
                {
                    Name = name,
                    NormalizedName = name.Trim().ToLowerInvariant()
                };
                
                db.Ingredients.Add(newIngredient);
            }
        }

        db.SaveChanges();

        return db.Ingredients
            .Where(x => names.Contains(x.Name))
            .ToList();        
    }
}