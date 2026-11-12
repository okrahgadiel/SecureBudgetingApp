# Secure Budgeting App

A secure, cloud-hosted personal finance web app that helps college students track income and expenses, set monthly budgets by category, and get alerted before they overspend.

Built as a **senior capstone project at Texas A&M University – Victoria (2026)**. Project concept, architecture, and team leadership by **Gadiel Okrah**.

## Features

- **Secure accounts** – registration and login with salted, hashed passwords (ASP.NET Core Identity `PasswordHasher`, PBKDF2) and cookie-based authentication with 30-minute sliding sessions
- **Per-user data isolation** – every query is scoped to the signed-in user's claims; no user can read another user's data
- **Transactions** – add, edit, and categorize income and expense transactions
- **Monthly budgets** – set spending limits per category per month
- **Interactive dashboard** – income, expenses, and net balance cards, per-category budget progress bars, and a Chart.js spending breakdown
- **Smart alerts** – automatic warnings as spending approaches or exceeds a category's limit, with read/unread tracking
- **Database-side reporting** – a SQL Server stored procedure (`GetMonthlyBudgetDashboard`) and view (`vw_MonthlyFinancialSummary`) do the aggregation server-side

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor (interactive server components), Razor, Bootstrap, Chart.js |
| Backend | C#, ASP.NET Core 8, minimal API endpoints |
| Data | Entity Framework Core 8, Azure SQL Database (SQL Server), stored procedures and views |
| Security | Cookie authentication, claims-based authorization, PBKDF2 password hashing, HTTPS/HSTS, antiforgery tokens |
| Cloud | Azure App Service, Azure SQL |

## Architecture

```
Browser ──HTTPS──▶ Azure App Service (ASP.NET Core 8 / Blazor Server)
                        │  cookie auth + claims (UserId)
                        ▼
                  EF Core 8 DbContext
                        │
                        ▼
                Azure SQL Database
   Users · Categories · Transactions · Budgets · Alerts
   vw_MonthlyFinancialSummary · GetMonthlyBudgetDashboard (stored proc)
```

## Running Locally

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download) and a SQL Server or Azure SQL database.

```bash
git clone https://github.com/<your-username>/SecureBudgetingApp.git
cd SecureBudgetingApp

# Store your connection string outside source control
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Initial Catalog=SecureBudgetingDB;User ID=...;Password=...;Encrypt=True;"

dotnet run
```

Credentials are never committed. `appsettings.json` contains only a placeholder, and the app reads the real connection string from .NET user secrets locally or from App Service configuration in Azure.

## Project Structure

```
Components/
  Layout/        Main layout and navigation
  Pages/         Home, Register, Login, Dashboard, Budget, AddTransaction, Alerts, About
Models/          EF Core entities, DbContext, stored-procedure mappings
wwwroot/         Static assets, Chart.js helper
Program.cs       Service registration, auth setup, login/logout endpoints
```

## Team

- **Gadiel Okrah** – Team Lead: project concept, architecture, and development
- Michael Njuguna – Contributor
- Sebastian Reyna – Contributor
