public sealed class Dish
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TasteRating { get; set; }
    public int SpentTimeMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<DishIngredient> DishIngredients { get; set; } = new();
    public List<RecipeStep> RecipeSteps { get; set; } = new();

}