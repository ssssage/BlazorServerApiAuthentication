using AdminPortal.Services;
using Microsoft.AspNetCore.Antiforgery;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TokenProvider : ITokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAntiforgery _antiforgery;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor)
    {
        // Get the named HttpClient "APIClient", which has BaseAddress set in Program.cs
        _httpClient = httpClientFactory.CreateClient("APIClient");
        _configuration = configuration;
        _antiforgery = antiforgery;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetTokenAsync(string email, string password)
    {
        var loginData = new { Email = email, Password = password };

        var response = await _httpClient.PostAsJsonAsync($"{_configuration["APISettings:ApiBaseUrl"]}/api/account/login", loginData);
        var responseContent = await response.Content.ReadAsStringAsync();  // Log response content
        Console.WriteLine($"API Response: {responseContent}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Login failed: {responseContent}");
        }

        // Deserialize correctly
        var result = JsonSerializer.Deserialize<LoginResult>(responseContent);

        return result?.Token;  // Return token if successful
    }
    public class LoginResult
    {
        [JsonPropertyName("token")]  // Map the 'token' field in the API response
        public string Token { get; set; }
    }
}
