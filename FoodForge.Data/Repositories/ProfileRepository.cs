using Microsoft.EntityFrameworkCore;

public sealed class ProfileRepository
{
    public List<Profile> GetAll()
    {
        using var db = FoodForgeDbContextProvider.Create();

        return db.Profile
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }
}