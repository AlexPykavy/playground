# Demo.EF.OptimisticLocking

## Generate migrations:
- `dotnet tool install --global dotnet-ef --version 6.0.27`
- `dotnet ef migrations add InitialCreate`

## References:
- https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
- https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#application-managed-concurrency-tokens
- https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/concurrency?view=aspnetcore-7.0#update-edit-methods
- https://dotnetcoretutorials.com/rowversion-vs-concurrencytoken-in-entityframework-efcore/
- https://www.bricelam.net/2020/08/07/sqlite-and-efcore-concurrency-tokens.html
