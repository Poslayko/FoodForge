public static class DatabasePaths
{
    public static string GetDatabasePath()
    {
        var appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);
        
        var appFolder = Path.Combine(appDataPath, "FoodForge");

        Directory.CreateDirectory(appFolder);

        return Path.Combine(appFolder, "foodforge.db");
    }

    public static string GetConnectionString()
    {
        var dbPath = GetDatabasePath();

        return $"Data Source={dbPath}";
    }
}