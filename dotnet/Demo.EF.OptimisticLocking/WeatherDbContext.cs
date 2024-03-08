using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Demo.EF.OptimisticLocking;

public class WeatherDbContext : DbContext
{
    public DbSet<WeatherForecast> Forecasts { get; set; }

    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }
}
