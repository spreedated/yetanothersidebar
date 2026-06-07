using Refit;
using Services.Models;
using System.Threading.Tasks;

namespace Services.RefitInterfaces
{
    public interface IUnifiApi
    {
        [Get("/proxy/network/api/s/default/stat/health")]
        Task<ApiResponse<string>> GetHealthAsync();

        [Get("/proxy/network/api/s/default/stat/sta")]
        Task<ApiResponse<string>> GetStatsAsync();

        [Get("/proxy/network/api/s/default/stat/device")]
        Task<ApiResponse<string>> GetDeviceAsync();

        [Post("/api/auth/login")]
        Task<ApiResponse<string>> LoginAsync([Body] LoginRequest credentials);
    }
}
