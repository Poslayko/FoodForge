using Microsoft.EntityFrameworkCore;

public static class FoodForgeDbContextProvider
{
    public static FoodForgeDbContext Create()
    {
        var options = new DbContextOptionsBuilder<FoodForgeDbContext>()
            .UseSqlite(DatabasePaths.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;

        return new FoodForgeDbContext(options);
    }
}