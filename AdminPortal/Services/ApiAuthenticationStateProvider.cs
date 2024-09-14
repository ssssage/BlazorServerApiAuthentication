using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;  // Added for logging
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AdminPortal.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        // Declare dependencies
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly NavigationManager _navigationManager;
        private readonly ILogger<ApiAuthenticationStateProvider> _logger;  // Added logger

        // Constructor with Dependency Injection
        public ApiAuthenticationStateProvider(
            HttpClient httpClient,
            ITokenProvider tokenProvider,
            NavigationManager navigationManager,
            ILogger<ApiAuthenticationStateProvider> logger)  // Injected logger
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _navigationManager = navigationManager;
            _logger = logger;  // Initialize logger
        }

        // Authenticate user with JWT token and claims
        public async Task MarkUserAsAuthenticated(string email, string token)
        {
            try
            {
                // Parse the JWT token and extract claims
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var claims = jwtToken.Claims.ToList();

                // Create a ClaimsPrincipal with the claims from the JWT token
                var identity = new ClaimsIdentity(claims, "apiauth");
                var user = new ClaimsPrincipal(identity);

                // Notify Blazor of the updated authentication state
                var authState = Task.FromResult(new AuthenticationState(user));
                NotifyAuthenticationStateChanged(authState);

                // Log authentication success
                _logger.LogInformation($"User {email} authenticated successfully with claims: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");

                // Set the Authorization header with the Bearer token for future API requests
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to authenticate user.");
                throw;
            }
        }

        // Log the user out and clear authentication state
        public void MarkUserAsLoggedOut()
        {
            // Create an anonymous ClaimsPrincipal (logged out user)
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);

            // Clear the Authorization header from HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Log the logout action
            _logger.LogInformation("User has logged out.");

            // Redirect the user to the homepage after logout
            _navigationManager.NavigateTo("/");
        }

        // Default implementation of the AuthenticationState
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Return an anonymous ClaimsPrincipal by default
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }
    }
}
