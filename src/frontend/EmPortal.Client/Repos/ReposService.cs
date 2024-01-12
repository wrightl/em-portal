using System.Net.Http;
using System.Text.Json;

namespace EmPortal.Client.Repos;

public class ReposService : IReposService
{
    private readonly HttpClient _httpClient;
    public ReposService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<WeatherForecast>> GetReposAsync()
    {
        var response = await _httpClient.GetAsync("/api/v1/repos/");

        if (!response.IsSuccessStatusCode)
            throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

        var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

}

interface IReposService
{
    Task<IEnumerable<WeatherForecast>> GetReposAsync();
}
