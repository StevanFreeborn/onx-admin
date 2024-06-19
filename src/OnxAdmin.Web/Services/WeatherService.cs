namespace OnxAdmin.Web.Services;

interface IWeatherService : IToolProvider
{
}

class WeatherService : ToolProvider, IWeatherService
{
  private static readonly string[] Summaries =
  [
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
  ];

  private static readonly string[] Cities =
  [
    "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose"
  ];

  [Function("This function allows you to the current weather forecast.")]
  public Task<string> GetForecastAsync(
    [FunctionParameter("This is a placeholder parameter. It is not needed to call the function.", false)] 
    object? _ = null
  )
  {
    var rng = new Random();
    var day = DateTime.Now.AddDays(rng.Next(1, 15));
    var temp = rng.Next(-20, 55);
    var summary = Summaries[rng.Next(Summaries.Length)];
    var city = Cities[rng.Next(Cities.Length)];
    var result = new WeatherForecast(day, temp, summary, city);
    var json = JsonSerializer.Serialize(result);
    return Task.FromResult(json);
  }
}

public record WeatherForecast(DateTime Date, int TemperatureC, string Summary, string City);