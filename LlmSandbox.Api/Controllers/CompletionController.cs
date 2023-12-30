using Microsoft.AspNetCore.Mvc;
using LlmSandbox.Api.Domains.Dtos;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Options;

namespace LlmSandbox.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompletionController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly OaiSettings _settings;

        public CompletionController(
            IHttpClientFactory clientFactory,
            IOptions<OaiSettings> options)
        {
            _httpClient = clientFactory.CreateClient();
            _settings = options.Value;
        }

        [HttpPost]
        [Route("vision")]
        public async Task<ActionResult<string>> PostAsync([FromBody] PostRequest request)
        {
            // base64エンコードされた画像を入れる時は data:image/jpeg;base64 から始めないといけない
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
	                ""text"": ""写真に写っているものを一言で説明してください。:""
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
            var baseUrl = $"https://{_settings.ResourceName}.openai.azure.com/openai/deployments/{_settings.DeploymentName}/chat/completions?api-version=2023-12-01-preview";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, baseUrl);
            httpRequest.Headers.Add("api-key", _settings.ApiKey);
            httpRequest.Content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
    }
}
