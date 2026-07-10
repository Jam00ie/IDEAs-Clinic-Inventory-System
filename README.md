# IDEAs Clinic Inventory System

An ASP.NET Core Razor Pages application for managing the University of Waterloo
IDEAs Clinic inventory. The application uses Entity Framework Core with Microsoft
SQL Server and is being developed as small, independently verifiable milestones.

## Current milestone

The completed foundation and first schema slice provide:

- .NET 10 Razor Pages web application
- Entity Framework Core configured for SQL Server
- SQL Server Express development configuration
- Process and database health endpoints
- xUnit integration test project
- Repository-pinned .NET SDK and `dotnet-ef` tool
- `CatalogItems` and `InventoryLocations` tables
- Initial EF Core migrations applied to the local development database
- Administrative create, view, edit, and delete pages for both tables

Tracked units and untracked inventory quantities will be added in a later milestone.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server Express instance named `SQLEXPRESS`
- Git
- Visual Studio Code with C# Dev Kit, or Visual Studio
- SQL Server Management Studio (optional)

Confirm the required services and tools from PowerShell:

```powershell
dotnet --version
Get-Service 'MSSQL$SQLEXPRESS'
```

The SDK should report `10.0.x`, and SQL Server should report `Running`.

## Restore and verify

From the repository root:

```powershell
dotnet tool restore
dotnet restore
dotnet tool run dotnet-ef database update --project src/IdeasClinicInventory.Web
dotnet build --no-restore
dotnet test --no-build
```

The tool manifest pins `dotnet-ef` to the same major and patch version as the EF
Core packages. This prevents different developer machines from silently using
incompatible migration tooling.

## Run locally

```powershell
dotnet run --project src/IdeasClinicInventory.Web
```

The development launch profile exposes:

- `https://localhost:7117`
- `http://localhost:5009`

If the browser does not trust the ASP.NET Core development certificate, run:

```powershell
dotnet dev-certs https --trust
```

## Database configuration

Development uses the `SQLEXPRESS` instance installed on the local Windows machine:

```text
Server=.\SQLEXPRESS
Database=IdeasClinicInventory
Authentication=Windows Authentication
```

The complete non-secret development connection string is in
`src/IdeasClinicInventory.Web/appsettings.Development.json`. It trusts the local
SQL Server self-signed certificate. Production must provide its connection string
through secure configuration and use the certificate settings supplied by
University of Waterloo IT.

To inspect the server in SQL Server Management Studio, connect to
`.\SQLEXPRESS` with Windows Authentication and select **Trust Server Certificate**.

## Health endpoints

- `/health` checks whether the web process can respond. It does not access SQL.
- `/health/ready` checks whether EF Core can connect to the inventory database.

After applying the migrations, both endpoints should report `Healthy` while SQL
Server Express is running.

## Administrative pages

After starting the application, use the **Admin** navigation menu or open:

- `/Admin/CatalogItems`
- `/Admin/InventoryLocations`

The pages validate input, report duplicate records, and use SQL Server rowversion
values to prevent one administrator from silently overwriting another's changes.
Authentication is not connected yet, so these routes are not currently secure.
An Entra-backed `AdminOnly` authorization policy must be applied before deployment.

## Repository structure

```text
IdeasClinicInventory.sln
global.json
dotnet-tools.json
src/
  IdeasClinicInventory.Web/
tests/
  IdeasClinicInventory.Tests/
```

Application code belongs under `src`; automated tests belong under `tests`.
