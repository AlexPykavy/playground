using Microsoft.AspNetCore.Mvc;

namespace Demo.ThreadStarvation.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await GenerateForecastWithDelay();
    }

    [HttpGet]
    [Route("taskwait")]
    public IEnumerable<WeatherForecast> TaskWait()
    {
        return GenerateForecastWithDelay(1000).Result;
    }

    [HttpGet]
    [Route("tasksleepwait")]
    public IEnumerable<WeatherForecast> TaskSleepWait()
    {
        var task = GenerateForecastWithDelay(1000);
        while(!task.IsCompleted)
        {
            Thread.Sleep(10);
        }
        return task.Result;
    }

    private static async Task<IEnumerable<WeatherForecast>> GenerateForecastWithDelay(int delay = 0)
    {
        if (delay > 0)
        {
            await Task.Delay(delay);
        }

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
