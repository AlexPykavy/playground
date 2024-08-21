using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Azure.MultipleSqlServers;

public class WeatherForecast
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    [ConcurrencyCheck]
    [Timestamp]
    [Column("row_version")]
    public byte[]? RowVersion { get; set; }
}
