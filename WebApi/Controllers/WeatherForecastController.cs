using csvUploadDomain;
using Microsoft.AspNetCore.Mvc;

namespace csvUploadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CallsController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<CallsController> _logger;

    public CallsController(ILogger<CallsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetCalls")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
