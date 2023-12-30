using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using LlmSandbox.Api.Domains.Dtos;
using System.Net.Mime;
using System.Text;

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
        private readonly HttpClient _httpClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _httpClient = clientFactory.CreateClient();
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
            var body = @$"{{
    ""messages"": [ 
        {{
            ""role"": ""system"", 
            ""content"": ""You are a helpful assistant."" 
        }},
        {{
            ""role"": ""user"", 
            ""content"": [
	            {{
	                ""type"": ""text"",
	                ""text"": ""写真の内容を説明して:""
	            }},
	            {{
	                ""type"": ""image_url"",
	                ""image_url"": {{
                        ""url"": ""{request.Image}""
                    }}
                }} 
           ] 
        }}
    ],
    ""max_tokens"": 100, 
    ""stream"": false 
}}";
            var resourceName = string.Empty;
            var deploymentName = string.Empty;
            var baseUrl = $"https://{resourceName}.openai.azure.com/openai/deployments/{deploymentName}/chat/completions?api-version=2023-12-01-preview";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, baseUrl);
            httpRequest.Headers.Add("api-key", string.Empty);
            httpRequest.Content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
    }

    public class PostRequest
    {
        public string Text { get; set; }

        public string Image { get; set; }
    }
}
