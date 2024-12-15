using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace API_Project.Controllers
{
    [Route("api/v{version:apiVersion}/gateway")]
    [ApiController]
    [ApiVersion("1.0")]
    public class GatewayController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GatewayController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Proxy for /api/v{version}/admin/report (GET)
        [HttpGet("admin/report")]
        public async Task<IActionResult> GetAdminReport(
            [FromQuery] string country,
            [FromQuery] string? city,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromRoute] string version = "1.0")
        {
            try
            {
                // Query string oluştur
                var queryString = $"?country={country}&city={city}&pageNumber={pageNumber}&pageSize={pageSize}";
                var apiUrl = $"https://localhost:7251/api/v{version}/admin/report{queryString}";

                // API'ye GET isteği gönder
                var response = await _httpClient.GetAsync(apiUrl);

                // Yanıtı kontrol et
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
                }

                // Dönen JSON'u al ve işleme
                var data = await response.Content.ReadAsStringAsync();
                return Content(data, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error accessing admin report: {ex.Message}");
            }
        }

        // Proxy for /api/v{version}/host/listings (POST)
        [HttpPost("host/listings")]
        public async Task<IActionResult> PostHostListing([FromBody] object listing, [FromRoute] string version = "1.0")
        {
            try
            {
                var apiUrl = $"https://localhost:7251/api/v{version}/host/listings";

                // API'ye POST isteği gönder
                var response = await _httpClient.PostAsJsonAsync(apiUrl, listing);

                // Yanıtı kontrol et
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
                }

                // Dönen JSON'u al ve işleme
                var data = await response.Content.ReadAsStringAsync();
                return Content(data, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error posting host listing: {ex.Message}");
            }
        }

        // Proxy for /api/Test/listings (GET)
        [HttpGet("test/listings")]
        public async Task<IActionResult> GetTestListings()
        {
            try
            {
                var apiUrl = "https://localhost:7251/api/Test/listings";

                // API'ye GET isteği gönder
                var response = await _httpClient.GetAsync(apiUrl);

                // Yanıtı kontrol et
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Error: {response.ReasonPhrase}");
                }

                // Dönen JSON'u al ve işleme
                var data = await response.Content.ReadAsStringAsync();
                return Content(data, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error accessing test listings: {ex.Message}");
            }
        }
    }
}
