public sealed class DishIngredient
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public Dish Dish { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public int Order { get; set; }
    public string Quantity { get; set; } = string.Empty;
    public string? MeasurementUnit { get; set; }
    public string? Comment { get; set; }
}