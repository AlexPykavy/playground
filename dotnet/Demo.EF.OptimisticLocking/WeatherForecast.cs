using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.EF.OptimisticLocking;

public class WeatherForecast
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    // https://stackoverflow.com/questions/19554050/entity-framework-6-code-first-default-value
    [ConcurrencyCheck]
    [Timestamp]
    [Column("row_version")]
    public int RowVersion { get; set; } = 0;
}