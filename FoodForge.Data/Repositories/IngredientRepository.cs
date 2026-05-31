public sealed class IngredientRepository
{
    public List<Ingredient> CreateIfNonExist(FoodForgeDbContext db, 
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

        return db.Ingredients
            .Where(x => names.Contains(x.Name))
            .ToList();        
    }
}