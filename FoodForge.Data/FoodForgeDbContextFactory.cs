using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class FoodForgeDbContextFactory 
    : IDesignTimeDbContextFactory<FoodForgeDbContext>
{
    public FoodForgeDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FoodForgeDbContext>()
            .UseSqlite(DatabasePaths.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;

        return new FoodForgeDbContext(options);
    }
}