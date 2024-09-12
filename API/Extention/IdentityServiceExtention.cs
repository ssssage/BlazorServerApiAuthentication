using System.Text;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // Configure Identity
            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<User>>();

            // Configure Token Service
            services.AddScoped<ITokenService, TokenService>();

            // Configure JWT Authentication
            var apiSettingsSection = config.GetSection("APISettings");
            services.Configure<APISettings>(apiSettingsSection);

            var apiSettings = apiSettingsSection.Get<APISettings>();
            var key = Encoding.ASCII.GetBytes(apiSettings.SecretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = apiSettings.ValidIssuer,
                        ValidAudience = apiSettings.ValidAudience,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Configure Authorization Policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireSellerRole", policy => policy.RequireRole("Seller"));
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin", "Seller"));
            });

            return services;
        }
    }
}
