public sealed class IngredientRepository
{
    public List<Ingredient> CreateIfNonExist(FoodForgeDbContext db, 
        List<string> names)
    {
        var existingIngredients = db.Ingredients
            .Where(x => names.Contains(x.Name))
            .ToList();

        var existingNames = existingIngredients
            .Select(x => x.Name)
            .ToHashSet();        

        var newIngredients = names
            .Where(name => !existingNames.Contains(name))
            .Select(name => new Ingredient
            {
                Name = name,
                NormalizedName = name.Trim().ToLowerInvariant()
            })
            .ToList();

        db.Ingredients.AddRange(newIngredients);

        return existingIngredients
            .Concat(newIngredients)
            .ToList();
    }
}