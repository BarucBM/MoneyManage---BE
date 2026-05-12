# MoneyManage API

Backend API for the MoneyManage personal finance application.

## Tech Stack
- **Framework:** .NET 10
- **Database:** PostgreSQL via Entity Framework Core (Npgsql)
- **API Style:** REST

## Key Features
- Account management (initial balance, tracking).
- Transaction management with automatic balance updates.
- Category-based financial tracking (Income/Expense).

## Getting Started
### Build
```bash
dotnet build -c Debug
```

### Run
```bash
dotnet run --project MoneyManage.Api.csproj
```

### Database Migrations
- Add migration: `dotnet ef migrations add <MigrationName>`
- Update database: `dotnet ef database update`
