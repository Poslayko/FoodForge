public sealed class RecipeStep
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public Dish Dish { get; set; } = null!;
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TimeMinutes { get; set; }
    public string? Comment { get; set; }
}