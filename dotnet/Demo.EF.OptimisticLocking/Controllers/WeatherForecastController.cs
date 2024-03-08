using Microsoft.AspNetCore.Mvc;

namespace Demo.EF.OptimisticLocking.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly WeatherDbContext _dbContext;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        WeatherDbContext dbContext,
        ILogger<WeatherForecastController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Select()
    {
        return _dbContext.Forecasts;
    }

    [HttpPost]
    public void Insert()
    {
        _dbContext.Add(new WeatherForecast
        {
            Date = DateTime.Now.AddDays(Random.Shared.Next(0, 5)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
        _dbContext.SaveChanges();
    }

    [HttpPut("{id}")]
    public void UpdateWithDelay(int id, int delay)
    {
        var forecast = _dbContext.Forecasts.First(f => f.Id == id);
        forecast.TemperatureC = Random.Shared.Next(-20, 55);
        forecast.Summary = Summaries[Random.Shared.Next(Summaries.Length)];

        Thread.Sleep(delay);

        _dbContext.SaveChanges();
    }
}
