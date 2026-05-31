using Microsoft.EntityFrameworkCore;

public sealed class DatabaseInitializer
{
    private readonly FoodForgeDbContext _dbContext;

    public DatabaseInitializer(FoodForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Initialize()
    {
        _dbContext.Database.Migrate();

        if(!_dbContext.Profiles.Any())
        {
            _dbContext.Profiles.Add(new Profile
            {
                Name = "Default"
            });
        }

        _dbContext.SaveChanges();
    }
}