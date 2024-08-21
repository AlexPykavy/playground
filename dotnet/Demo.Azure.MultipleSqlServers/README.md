# Demo.EF.Expressions

## Generate migrations:
- `dotnet tool install --global dotnet-ef --version 6.0.27`
- `dotnet ef migrations add InitialCreate --context WeatherContextMainSqlServer`
- `dotnet ef database update --context WeatherContextMainSqlServer`
