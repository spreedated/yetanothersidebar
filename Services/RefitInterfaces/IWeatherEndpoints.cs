using Refit;
using Services.Models;
using System.Threading.Tasks;

namespace Services.RefitInterfaces
{
    public interface IWeatherEndpoints
    {
        [Get("/forecast.json?key={apiKey}&q=Heinsberg&days=3&aqi=yes")]
        [Headers("User-Agent: YetAnotherMonitor/v2.0.0", "Accept: application/json")]
        Task<WeatherApi> GetWeatherAsync(string apiKey);
    }
}
