using AdminPortal.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Logging;  // Added for robust logging
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TokenProvider : ITokenProvider
{
    // Declare dependencies to be injected later
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAntiforgery _antiforgery;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TokenProvider> _logger;  // Added logger

    // Constructor with Dependency Injection
    public TokenProvider(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IAntiforgery antiforgery,
        IHttpContextAccessor httpContextAccessor,
        ILogger<TokenProvider> logger)  // Injected logger
    {
        _httpClient = httpClientFactory.CreateClient("APIClient");
        _configuration = configuration;
        _antiforgery = antiforgery;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;  // Logger initialized
    }

    // Get JWT token by posting login data to the API
    public async Task<string> GetTokenAsync(string email, string password)
    {
        var loginData = new { Email = email, Password = password };

        try
        {
            // Post the login data to the API and receive the response
            var response = await _httpClient.PostAsJsonAsync($"{_configuration["APISettings:ApiBaseUrl"]}/api/account/login", loginData);
            var responseContent = await response.Content.ReadAsStringAsync();  // Read the API response

            // Log response for debugging
            _logger.LogInformation($"API Response: {responseContent}");

            // Check if the response was successful, else log and throw an exception
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Login failed with status code: {response.StatusCode}, Response: {responseContent}");
                throw new HttpRequestException($"Login failed with status code: {response.StatusCode}");
            }

            // Deserialize response content to LoginResult
            var result = JsonSerializer.Deserialize<LoginResult>(responseContent);

            // Return the token if successful, else log and return null
            if (result?.Token == null)
            {
                _logger.LogWarning("Token was null after successful login.");
                return null;
            }

            return result.Token;  // Return JWT token
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An error occurred during login.");
            throw;
        }
    }

    // Class to hold the deserialized response
    public class LoginResult
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
