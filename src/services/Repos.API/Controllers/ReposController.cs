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

    public ReposController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), (int)HttpStatusCode.OK)]
    public async Task<IEnumerable<WeatherForecast>> GetRepos()
    {
        var cachedForecast = await _cache.GetAsync("forecast");

        if (cachedForecast is null)
        {
            var summaries = new[]
                {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            var forecast = Enumerable.Range(1, 5).Select(index =>
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

            return forecast;
        }
        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedForecast);
    }
}