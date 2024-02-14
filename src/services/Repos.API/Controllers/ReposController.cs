using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace EmPortal.Repos.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ReposController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ReposController> _logger;

    public ReposController(IDistributedCache cache, ILogger<ReposController> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), (int)HttpStatusCode.OK)]
    public async Task<IEnumerable<WeatherForecast>> GetRepos()
    {
        _logger.LogInformation("Entering GetRepos...");
        var cachedForecast = await _cache.GetAsync("forecast");
        IEnumerable<WeatherForecast> forecast;

        if (cachedForecast is null)
        {
            _logger.LogInformation("No cached forecasts, getting latest");
            var summaries = new[]
                {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                    .ToArray();

            await _cache.SetAsync("forecast", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(forecast)), new()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10)
            });
        }
        else
        {
            _logger.LogInformation("Using cached forecasts");
            forecast = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedForecast);
        }
        return forecast;
    }
}