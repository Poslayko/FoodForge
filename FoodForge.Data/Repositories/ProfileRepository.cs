using Microsoft.EntityFrameworkCore;

public sealed class ProfileRepository
{
    private readonly IDbContextFactory<FoodForgeDbContext> _dbContextFactory;

    public ProfileRepository(IDbContextFactory<FoodForgeDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    public List<Profile> GetAll()
    {
        using var db = _dbContextFactory.CreateDbContext();

        return db.Profiles
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }
}