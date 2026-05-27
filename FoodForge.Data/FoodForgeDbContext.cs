using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class FoodForgeDbContext : DbContext
{
    public DbSet<Profile> Profile => Set<Profile>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<DishIngredient> DishIngredients => Set<DishIngredient>();
    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();

    public FoodForgeDbContext(DbContextOptions<FoodForgeDbContext> options) 
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(profile => profile.Id);

            entity.Property(profile => profile.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Profile>()
            .HasMany(profile => profile.Dishes)
            .WithOne(dish => dish.Profile)
            .HasForeignKey(dish => dish.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(dish => dish.Id);

            entity.Property(dish => dish.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(dish => dish.Description)
                .HasMaxLength(800);

        });

        modelBuilder.Entity<Dish>()
            .HasMany(dish => dish.RecipeSteps)
            .WithOne(recipeStep => recipeStep.Dish)
            .HasForeignKey(recipeStep => recipeStep.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DishIngredient>(entity =>
        {
            entity.Property(dishIngredient => dishIngredient.Order)
                .IsRequired();
        });

        modelBuilder.Entity<DishIngredient>()
            .HasOne(dish => dish.Dish)
            .WithMany(dish => dish.DishIngredients)
            .HasForeignKey(dish => dish.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(ingredient => ingredient.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<DishIngredient>()
            .HasOne(ingredient => ingredient.Ingredient)
            .WithMany(ingredient => ingredient.DishIngredients)
            .HasForeignKey(ingredient => ingredient.IngredientId);

        modelBuilder.Entity<RecipeStep>(entity =>
        {
            entity.Property(recipeStep => recipeStep.Description)
                .IsRequired()
                .HasMaxLength(400);

            entity.Property(recipeStep => recipeStep.Order)
                .IsRequired();

            entity.Property(recipeStep => recipeStep.Comment)
                .IsRequired(false);
        });
    }
}
