public sealed record class FullEditingDish
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int TasteRating { get; set; }
    public int SpentTimeMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<FullDishIngredient> Ingredients { get; set; } = new();
    public List<RecipeStep> RecipeSteps { get; set; } = new();
}