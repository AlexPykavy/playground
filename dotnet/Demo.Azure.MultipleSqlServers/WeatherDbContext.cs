using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Azure.MultipleSqlServers;

public class WeatherMainDbContext : WeatherDbContext {}
public class WeatherReadOnlyDbContext : WeatherDbContext {}

public class WeatherDbContext : DbContext
{
    public DbSet<WeatherForecast> Forecasts { get; set; }
}
