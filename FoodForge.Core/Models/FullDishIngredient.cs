public sealed class FullDishIngredient : IOrderedItem
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public int IngredientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }
    public string Quantity { get; set; } = string.Empty;
    public string? MeasurementUnit { get; set; }
    public string? Comment { get; set; }

}