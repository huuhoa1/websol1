# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

1. First think through the problem, read the codebase for relevant files, and write a plan to todo.md.
2. The plan should have a list of todo items that you can check off as you complete them
3. Before you begin working, check in with me and I will verify the plan.
4. Then, begin working on the todo items, marking them as complete as you go.
5. Please every step of the way just give me a high level explanation of what changes you made
6. Make every task and code change you do as simple as possible. We want to avoid making any massive or complex changes. Every change should impact as little code as possible. Everything is about simplicity.
7. Finally, add a review section to the todo.md file with a summary of the changes you made and any other relevant information.

## Project Overview

This is a standard ASP.NET Core 8.0 MVC web application (`webapp1`) using the default template structure. The application uses minimal APIs with a traditional MVC pattern for web pages.

## Development Commands

### Build and Run
```bash
# Build the application
dotnet build

# Run in development mode
dotnet run

# Run with specific profile
dotnet run --launch-profile https
```

### Testing
```bash
# Run tests (if test projects are added)
dotnet test
```

### Package Management
```bash
# Restore NuGet packages
dotnet restore

# Add new package
dotnet add package <PackageName>
```

## Architecture

- **Framework**: ASP.NET Core 8.0 MVC
- **Target Framework**: .NET 8.0
- **Features Enabled**: Nullable reference types, implicit usings
- **Dependency Injection**: Built-in ASP.NET Core DI container

### Project Structure
- `Controllers/`: MVC controllers (currently HomeController with Index, Privacy, Error actions)
- `Models/`: View models and data models (currently ErrorViewModel)
- `Views/`: Razor views organized by controller
- `wwwroot/`: Static web assets (CSS, JS, images, client libraries)
- `Program.cs`: Application entry point and configuration
- `appsettings.json`: Application configuration

### Configuration
- Development server runs on `https://localhost:7177` and `http://localhost:5053`
- IIS Express runs on `http://localhost:21292` with SSL port `44361`
- Uses default ASP.NET Core logging configuration

## Key Patterns

- Standard MVC routing: `{controller=Home}/{action=Index}/{id?}`
- Dependency injection through constructor injection (see HomeController)
- Conventional folder structure following ASP.NET Core defaults
- Client libraries managed in `wwwroot/lib/` (Bootstrap, jQuery, jQuery Validation)