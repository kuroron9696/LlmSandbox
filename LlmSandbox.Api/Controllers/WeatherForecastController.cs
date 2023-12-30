using LlmSandbox.Api.Domains.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LlmSandbox.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [Route("/file")]
        public async Task<ActionResult<string>> PostAsync([FromBody] PostRequest request)
        {
            var response = request.Text;
            var bytes = Convert.FromBase64String(request.Image);
            MemoryStream stream = new MemoryStream(bytes);
            var file = new FormFile(stream, 0, bytes.Length, "test", "test");
            return Ok(response + file.Name);
        }
    }

    public class PostRequest
    {
        public string Text { get; set; }

        public string Image { get; set; }
    }
}
