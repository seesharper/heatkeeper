# HeatKeeper Project Patterns Guide for AI Agents

This document describes the architectural patterns, conventions, and practices used in the HeatKeeper project to help AI coding agents understand and maintain consistency when making changes.

## Project Overview

HeatKeeper is a .NET 9.0 heating control system with a CQRS (Command Query Responsibility Segregation) architecture, built with ASP.NET Core and SQLite.

## Architecture Patterns

### 1. CQRS Pattern

The project strictly separates commands (write operations) from queries (read operations).

#### Queries (Read Operations)

**Pattern:**
```csharp
namespace HeatKeeper.Server.[Domain].Api;

[RequireUserRole]  // or [RequireAdminRole], [RequireBackgroundRole]
[Get("api/[resource]/{id}")]
public record [Name]Query(parameters...) : IQuery<ResultType>;

public record ResultType(properties...);

public class [Name](IDbConnection dbConnection, ISqlProvider sqlProvider) 
    : IQueryHandler<[Name]Query, ResultType>
{
    public async Task<ResultType> HandleAsync([Name]Query query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ResultType>(sqlProvider.[SqlPropertyName], query)).Single();
}
```

**Example:**
```csharp
[RequireUserRole]
[Get("api/heaters/{heaterId}")]
public record HeaterDetailsQuery(long HeaterId) : IQuery<HeaterDetails>;

public record HeaterDetails(long Id, string Name, string ZoneName, string Description, string MqttTopic, string OnPayload, string OffPayload, bool Enabled);

public class GetHeaterDetails(IDbConnection dbConnection, ISqlProvider sqlProvider) 
    : IQueryHandler<HeaterDetailsQuery, HeaterDetails>
{
    public async Task<HeaterDetails> HandleAsync(HeaterDetailsQuery query, CancellationToken cancellationToken = default) =>
        (await dbConnection.ReadAsync<HeaterDetails>(sqlProvider.GetHeaterDetails, query)).Single();
}
```

#### Commands (Write Operations)

**API Commands Pattern (Web API Endpoints):**
```csharp
namespace HeatKeeper.Server.[Domain].Api;

[RequireAdminRole]  // or [RequireUserRole]
[Patch("api/[resource]/{id}")]  // or [Post], [Delete]
public record [Name]Command(parameters...) : PatchCommand;  // or PostCommand, DeleteCommand

public class [Name](IDbConnection dbConnection, ISqlProvider sqlProvider) 
    : ICommandHandler<[Name]Command>
{
    public async Task HandleAsync([Name]Command command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.[SqlPropertyName], command);
}
```

**Background Commands Pattern (No Web API):**
```csharp
namespace HeatKeeper.Server.[Domain];

[RequireBackgroundRole]
public record [Name]Command(parameters...);

public class [Name](IDbConnection dbConnection, ISqlProvider sqlProvider) 
    : ICommandHandler<[Name]Command>
{
    public async Task HandleAsync([Name]Command command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.[SqlPropertyName], command);
}
```

**Example:**
```csharp
[RequireBackgroundRole]
public record EnableHeaterCommand(long HeaterId);

public class EnableHeater(IDbConnection dbConnection, ISqlProvider sqlProvider) 
    : ICommandHandler<EnableHeaterCommand>
{
    public async Task HandleAsync(EnableHeaterCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.EnableHeater, command);
}
```

### 2. Authorization Roles

Three role levels control access:

- **`[RequireAdminRole]`**: Administrative operations (CRUD on resources)
- **`[RequireUserRole]`**: Standard user operations (read, some modifications)
- **`[RequireBackgroundRole]`**: Background/system operations (not exposed via API)

**Location:** Applied as attributes on command/query records.

### 3. Database Layer

#### SQL Files Pattern

SQL queries are embedded resources stored in `.sql` files alongside the code:

**Location:** `/src/HeatKeeper.Server.Database/[Domain]/[OperationName].sql`

**Example:**
```sql
-- File: /src/HeatKeeper.Server.Database/Heaters/EnableHeater.sql
UPDATE
    Heaters
SET
    Enabled = 1
WHERE
    Id = @HeaterId
```

#### SQL Provider Interface

All SQL queries are accessed through the `ISqlProvider` interface:

**Location:** `/src/HeatKeeper.Server.Database/Sql.cs`

**Pattern:**
```csharp
public interface ISqlProvider
{
    string [OperationName] { get; }
    // ...
}
```

**Important:** Property names in `ISqlProvider` must exactly match the SQL filename (without `.sql` extension).

#### Database Migrations

**Location:** `/src/HeatKeeper.Server.Database/Migrations/` or `/Migrations/Version[N]/`

**Pattern:**
```csharp
using System.Data;
using DbReader;
using HeatKeeper.Server.Database.Migrations;

namespace HeatKeeper.Server.Database.Migrations.Version[N];

[AppliesToVersion([N], Order = [X])]
public class [MigrationName](ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.[MigrationName]);
    }
}
```

**Migration SQL File:** `/src/HeatKeeper.Server.Database/[MigrationName].sql`

**Example:**
```csharp
[AppliesToVersion(15, Order = 2)]
public class AddEnabledColumnToHeatersTable(ISqlProvider sqlProvider) : IMigration
{
    public void Migrate(IDbConnection dbConnection)
    {
        dbConnection.Execute(sqlProvider.AddEnabledColumnToHeatersTable);
    }
}
```

```sql
-- AddEnabledColumnToHeatersTable.sql
ALTER TABLE Heaters ADD COLUMN Enabled INTEGER NOT NULL DEFAULT 1;
```

### 4. Event Actions

Event-driven actions for automation and triggers.

**Location:** `/src/HeatKeeper.Server/Events/`

**Pattern:**
```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HeatKeeper.Server.Events;

public record [Name]ActionParameters(
    [property: Description("Description"), Required] Type ParameterName);

[Action([UniqueId], "[Display Name]", "[Description]")]
public sealed class [Name]Action(ICommandExecutor commandExecutor) 
    : IAction<[Name]ActionParameters>
{
    public async Task ExecuteAsync([Name]ActionParameters parameters, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new [Command](parameters.ParameterName), cancellationToken);
}
```

**Example:**
```csharp
public record EnableHeaterActionParameters(
    [property: Description("The ID of the heater to enable"), Required] long HeaterId);

[Action(4, "Enable Heater", "Enables a specific heater")]
public sealed class EnableHeaterAction(ICommandExecutor commandExecutor) 
    : IAction<EnableHeaterActionParameters>
{
    public async Task ExecuteAsync(EnableHeaterActionParameters parameters, CancellationToken cancellationToken = default)
        => await commandExecutor.ExecuteAsync(new EnableHeaterCommand(parameters.HeaterId), cancellationToken);
}
```

## File Organization

### Directory Structure

```
src/
├── HeatKeeper.Server/
│   ├── [Domain]/
│   │   ├── Api/                    # Web API endpoints (commands/queries)
│   │   │   ├── Get[Resource].cs
│   │   │   ├── Create[Resource].cs
│   │   │   ├── Update[Resource].cs
│   │   │   └── Delete[Resource].cs
│   │   └── [BackgroundCommand].cs  # Background operations
│   ├── Events/                     # Event actions
│   └── Authorization/              # Authorization attributes
├── HeatKeeper.Server.Database/
│   ├── [Domain]/                   # SQL files by domain
│   │   ├── Get[Resource].sql
│   │   ├── Insert[Resource].sql
│   │   ├── Update[Resource].sql
│   │   └── Delete[Resource].sql
│   ├── Migrations/                 # Database migrations
│   │   ├── Version[N]/
│   │   └── [MigrationName].cs
│   ├── [MigrationName].sql         # Migration SQL files
│   └── Sql.cs                      # ISqlProvider interface
└── HeatKeeper.Server.WebApi.Tests/
    └── [Domain]Tests.cs
```

### Naming Conventions

#### Commands
- **API Commands:** `[Verb][Resource]Command` (e.g., `UpdateHeaterCommand`, `CreateSensorCommand`)
- **Background Commands:** `[Verb][Resource]Command` (e.g., `EnableHeaterCommand`)
- **Command Handlers:** `[Verb][Resource]` (e.g., `UpdateHeater`, `EnableHeater`)

#### Queries
- **Query Records:** `[Resource]Query` or `[Resource]DetailsQuery`
- **Query Handlers:** `Get[Resource]` or `Get[Resource]Details`
- **Result Records:** `[Resource]Info`, `[Resource]Details`, or custom name

#### SQL Files
- **Queries:** `Get[Resource].sql`, `Get[Resource]Details.sql`
- **Commands:** `Insert[Resource].sql`, `Update[Resource].sql`, `Delete[Resource].sql`, `[Action][Resource].sql`
- **Migrations:** `[Description].sql` (e.g., `AddEnabledColumnToHeatersTable.sql`)

#### Migrations
- Class: `[Description]` (e.g., `AddEnabledColumnToHeatersTable`)
- SQL File: Same as class name with `.sql` extension

## Code Style

### Primary Constructor Injection

The project uses C# primary constructors for dependency injection:

```csharp
public class ServiceName(IDependency dependency, IAnotherDependency another)
{
    // Dependencies are automatically available as fields
}
```

### Records for DTOs

Use records for immutable data transfer objects:

```csharp
public record ResultType(long Id, string Name, bool IsActive);
```

### Expression-Bodied Members

Use expression-bodied members for simple operations:

```csharp
public async Task<ResultType> HandleAsync(Query query, CancellationToken cancellationToken = default)
    => (await dbConnection.ReadAsync<ResultType>(sqlProvider.SqlProperty, query)).Single();
```

### Parameter Attributes

For action parameters, use property attributes:

```csharp
public record Parameters(
    [property: Description("Description"), Required] Type Name);
```

## Testing Patterns

**Location:** `/src/HeatKeeper.Server.WebApi.Tests/`

Test files follow the pattern `[Domain]Tests.cs` and use xUnit framework.

## Common Workflows

### Adding a New Command/Query to Existing Domain

1. **Create the handler class** in `/src/HeatKeeper.Server/[Domain]/` or `/src/HeatKeeper.Server/[Domain]/Api/`
2. **Create the SQL file** in `/src/HeatKeeper.Server.Database/[Domain]/[OperationName].sql`
3. **Add SQL property** to `ISqlProvider` in `/src/HeatKeeper.Server.Database/Sql.cs`
4. **Build the project** to regenerate SQL provider implementation
5. **Add tests** in `/src/HeatKeeper.Server.WebApi.Tests/[Domain]Tests.cs`

### Adding a Database Migration

1. **Create migration SQL file** in `/src/HeatKeeper.Server.Database/[MigrationName].sql`
2. **Create migration class** in `/src/HeatKeeper.Server.Database/Migrations/Version[N]/[MigrationName].cs`
3. **Add SQL property** to `ISqlProvider` in `/src/HeatKeeper.Server.Database/Sql.cs`
4. **Update version number** if starting a new version
5. **Build and test** the migration

### Adding a New Event Action

1. **Create action class** in `/src/HeatKeeper.Server/Events/[Name]Action.cs`
2. **Define parameters record** with appropriate attributes
3. **Apply `[Action]` attribute** with unique ID, name, and description
4. **Inject `ICommandExecutor`** if executing commands
5. **Build and test** the action

## Important Notes

### SQL Parameter Binding

- SQL files use `@ParameterName` for parameter placeholders
- Parameter names must match command/query record property names (case-insensitive)

### SQL File as Embedded Resources

All `.sql` files are embedded resources (configured in `.csproj`):

```xml
<ItemGroup>
  <EmbeddedResource Include="**/*.sql" />
</ItemGroup>
```

### Authorization Context

- **API endpoints** require `[RequireAdminRole]` or `[RequireUserRole]`
- **Background operations** require `[RequireBackgroundRole]`
- Commands/queries are automatically authorized by the framework

### ISqlProvider Implementation

The `ISqlProvider` interface is automatically implemented at runtime using the `ResourceReader` package, which loads SQL files as embedded resources. Property names must exactly match SQL filenames.

## Technology Stack

- **.NET 9.0**
- **ASP.NET Core** (Web API)
- **SQLite** (Database)
- **DbReader** (Data access)
- **CQRS.Command.Abstractions** (CQRS framework)
- **MQTTnet** (MQTT messaging)
- **xUnit** (Testing)

## Build and Test

- **Build:** `dotnet build`
- **Test:** `dotnet test` or use the configured tasks
- **Tasks available:** build, rebuild, test, test with coverage, docker image

## Key Principles

1. **Separation of Concerns:** Commands, queries, and handlers are separate
2. **Single Responsibility:** Each handler does one thing
3. **Dependency Injection:** All dependencies injected via constructor
4. **Immutability:** Use records for DTOs
5. **Security:** Always specify authorization roles
6. **SQL as Resources:** Keep SQL separate from C# code
7. **Convention over Configuration:** Follow naming patterns consistently
