using AdminPortal.Components;
using AdminPortal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AdminPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents() // Register Razor Components services
                .AddInteractiveServerComponents(); // Add support for interactive server-side rendering

            // Register the configuration section for API settings
            builder.Services.Configure<APISettings>(builder.Configuration.GetSection("APISettings"));

            // Register HttpClient for making API calls with a named client 'APIClient'
            builder.Services.AddHttpClient("APIClient")
                            .ConfigureHttpClient(c =>
                                c.BaseAddress = new Uri(builder.Configuration["APISettings:ApiBaseUrl"] ?? "")); // Set base URL for HttpClient

            // Register Server-Side Blazor services
            builder.Services.AddServerSideBlazor();

            // Add JWT-based authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default authentication scheme to JWT
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;   // Set the default challenge scheme to JWT
            })
            .AddJwtBearer(options =>
            {
                // Configure JWT token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,  // Ensure the token is issued by a valid authority
                    ValidateAudience = true, // Ensure the token is intended for a valid audience
                    ValidateLifetime = true, // Ensure the token has not expired
                    ValidateIssuerSigningKey = true, // Ensure the token is signed with the correct key
                    ValidIssuer = builder.Configuration["APISettings:ValidIssuer"], // Issuer from config
                    ValidAudience = builder.Configuration["APISettings:ValidAudience"], // Audience from config
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["APISettings:SecretKey"])) // Secret key from config
                };

                // Handle unauthenticated requests and redirect to home page
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse(); // Prevent default 401 behavior
                        context.Response.Redirect("/"); // Redirect to home page for unauthorized access
                        return Task.CompletedTask;
                    }
                };
            });

            // Register Antiforgery service for CSRF protection
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN"; // Set custom header for the antiforgery token
            });

            // Register AuthorizationCore for managing component-level authorization
            builder.Services.AddAuthorizationCore();

            // Register IHttpContextAccessor for accessing the current HTTP context
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register TokenProvider as ITokenProvider for handling tokens
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();

            // Register the custom ApiAuthenticationStateProvider for managing authentication state
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

            // Build the web application
            var app = builder.Build();

            // Configure error handling for production environments
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error"); // Redirect to custom error page
                app.UseHsts(); // Enable HTTP Strict Transport Security (HSTS)
            }

            app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS

            app.UseStaticFiles(); // Serve static files (CSS, JS, etc.)

            app.UseRouting(); // Enable routing in the app

            app.UseAuthentication(); // Enable JWT authentication middleware

            app.UseAuthorization(); // Enable authorization middleware

            // Add Antiforgery Middleware to validate CSRF tokens on form submissions
            app.UseAntiforgery();

            // Custom fallback to handle non-existent routes
            app.MapFallback(context =>
            {
                var isAuthenticated = context.User.Identity.IsAuthenticated;

                if (!isAuthenticated)
                {
                    // Redirect unauthenticated users to login page for any invalid URL
                    context.Response.Redirect("/login");
                }
                else
                {
                    // Redirect authenticated users to home page for invalid URLs
                    context.Response.Redirect("/");
                }

                return Task.CompletedTask;
            });

            // Map Razor Components with support for interactive server rendering
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Run the application
            app.Run();
        }
    }
}
