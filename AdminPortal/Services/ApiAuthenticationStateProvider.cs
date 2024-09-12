using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AdminPortal.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly NavigationManager _navigationManager;

        public ApiAuthenticationStateProvider(HttpClient httpClient, ITokenProvider tokenProvider, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _navigationManager = navigationManager;
        }

        // Align this method with the TokenService claims handling
        public async Task MarkUserAsAuthenticated(string email, string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = jwtToken.Claims.ToList();

            // Create the ClaimsPrincipal
            var identity = new ClaimsIdentity(claims, "apiauth");
            var user = new ClaimsPrincipal(identity);

            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);

            Console.WriteLine("Token received: " + token);
            Console.WriteLine("User is being authenticated with claims: " + string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));


            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void MarkUserAsLoggedOut()
        {
            // Clear the authentication state when the user logs out
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);

            // Remove the Authorization header from HttpClient
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Navigate to home page after logout
           _navigationManager.NavigateTo("/");
        }

        // Default implementation when getting the current authentication state
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        }
    }
}
