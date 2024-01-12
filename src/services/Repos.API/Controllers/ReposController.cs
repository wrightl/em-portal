using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace EmPortal.Repos.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ReposController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), (int)HttpStatusCode.OK)]
    public ActionResult<IEnumerable<WeatherForecast>> GetRepos()
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
        return Ok(forecast);
    }
}