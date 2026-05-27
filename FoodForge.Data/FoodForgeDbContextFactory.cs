using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class FoodForgeDbContextFactory 
    : IDesignTimeDbContextFactory<FoodForgeDbContext>
{
    public FoodForgeDbContext CreateDbContext(string[] args)
    {
        return FoodForgeDbContextProvider.Create();
    }
}