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
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Register the configuration section
            builder.Services.Configure<APISettings>(builder.Configuration.GetSection("APISettings"));

            builder.Services.AddHttpClient("APIClient")
                            .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["APISettings:ApiBaseUrl"] ?? ""));
                      
            builder.Services.AddServerSideBlazor();
            // Add authentication services
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = builder.Configuration["APISettings:ValidIssuer"],
                         ValidAudience = builder.Configuration["APISettings:ValidAudience"],
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["APISettings:SecretKey"]))
                     };
                     // Handle unauthenticated requests and redirect to home page
                     options.Events = new JwtBearerEvents
                     {
                         OnChallenge = context =>
                         {
                             // Skip the default logic to avoid returning a 401 response
                             context.HandleResponse();

                             // Redirect unauthenticated users to the home page
                             context.Response.Redirect("/");
                             return Task.CompletedTask;
                         }
                     };
                 });
        

            // Antiforgery configuration
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            // Add authorization services
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Add Antiforgery Middleware
            app.UseAntiforgery();
       
            app.MapFallback(context =>
            {
                var isAuthenticated = context.User.Identity.IsAuthenticated;

                if (!isAuthenticated)
                {
                    // Redirect unauthenticated users to login page if they try to access any non-existing route
                    context.Response.Redirect("/login");
                }
                else
                {
                    // Redirect authenticated users to home page if they try to access non-existing routes
                    context.Response.Redirect("/");
                }

                return Task.CompletedTask;
            });

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }

}