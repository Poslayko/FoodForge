public sealed class Profile
{
    public int Id { get; set; }
    public string Name { get; set; } = "Default";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Dish> Dishes { get; set; } = new();
}