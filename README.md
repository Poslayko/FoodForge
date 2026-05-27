# FoodForge

FoodForge is a desktop application for managing dishes, ingredients, and recipe steps.

## Features

- Create and edit dishes.
- Add, edit, delete, and reorder ingredients.
- Add, edit, delete, and reorder recipe steps.
- Store dish details such as description, taste rating, and spent time.
- Persist data locally with SQLite and Entity Framework Core.

## Tech Stack

- C# / .NET 9
- Avalonia UI
- Entity Framework Core
- SQLite

## Project Structure

- `FoodForge.App` - Avalonia desktop app, windows, and view models.
- `FoodForge.Core` - domain entities and shared models.
- `FoodForge.Data` - EF Core `DbContext`, migrations, repositories, services, and database initialization.

## Getting Started

### Requirements

- .NET 9 SDK

### Run the App

```bash
dotnet run --project FoodForge.App
```

## Database

FoodForge uses a local SQLite database. The database is created and migrated automatically on application startup.

The database path is built from the operating system's local application data folder:

```text
<LocalApplicationData>/FoodForge/foodforge.db
```

Examples:

```text
Windows: C:\Users\<User>\AppData\Local\FoodForge\foodforge.db
macOS:   /Users/<User>/Library/Application Support/FoodForge/foodforge.db
Linux:   /home/<User>/.local/share/FoodForge/foodforge.db
```

The app creates the `FoodForge` folder automatically if it does not exist.

## EF Core Commands

Create a migration:

```bash
dotnet ef migrations add MigrationName --project FoodForge.Data --startup-project FoodForge.App
```

Apply migrations manually:

```bash
dotnet ef database update --project FoodForge.Data --startup-project FoodForge.App
```

Manual migration is usually not required for normal app startup because `DatabaseInitializer` runs migrations automatically.

## Notes

- `FullEditingDish` is used as an editable model for the UI.
- Editable UI collections are handled in the ViewModel with `ObservableCollection`.
- EF Core entities are stored in `FoodForge.Core/Entities`.
