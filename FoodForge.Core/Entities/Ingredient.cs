public sealed class Ingredient
{
    public int Id { get; set; }
    public List<DishIngredient> DishIngredients { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
}