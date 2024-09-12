using API.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Seller"))
            {
                await roleManager.CreateAsync(new IdentityRole("Seller"));
            }

            // Seed Admin Users
            var adminUser1 = new User
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                DisplayName = "Admin"
            };

            if (await userManager.FindByEmailAsync(adminUser1.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser1, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser1, "Admin");
                }
            }

            var adminUser2 = new User
            {
                UserName = "admin2@test.com",
                Email = "admin2@test.com",
                DisplayName = "AdminTwo"
            };

            if (await userManager.FindByEmailAsync(adminUser2.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser2, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser2, "Admin");
                }
            }

            // Seed Seller Users
            var sellerUser1 = new User
            {
                UserName = "seller@test.com",
                Email = "seller@test.com",
                DisplayName = "SellerOne",
                IsSeller = true
            };

            if (await userManager.FindByEmailAsync(sellerUser1.Email) == null)
            {
                var result = await userManager.CreateAsync(sellerUser1, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(sellerUser1, "Seller");

                    var context = serviceProvider.GetRequiredService<AppDbContext>();

                    // Seed Products
                    context.Products.AddRange(new List<Product>
                    {
                        new Product { Name = "iPhone 12", Description = "Apple iPhone 12", Price = 799, Type = "Phone", Brand = "Apple", SellerId = sellerUser1.Id, MainImageUrl = "url_to_image" },
                        new Product { Name = "Galaxy S21", Description = "Samsung Galaxy S21", Price = 999, Type = "Phone", Brand = "Samsung", SellerId = sellerUser1.Id, MainImageUrl = "url_to_image" },
                    });

                    await context.SaveChangesAsync();
                }
            }

            var sellerUser2 = new User
            {
                UserName = "seller2@test.com",
                Email = "seller2@test.com",
                DisplayName = "SellerTwo",
                IsSeller = true
            };

            if (await userManager.FindByEmailAsync(sellerUser2.Email) == null)
            {
                var result = await userManager.CreateAsync(sellerUser2, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(sellerUser2, "Seller");

                    var context = serviceProvider.GetRequiredService<AppDbContext>();

                    // Seed more Products
                    context.Products.AddRange(new List<Product>
                    {
                        new Product { Name = "MacBook Pro", Description = "Apple MacBook Pro", Price = 1999, Type = "Laptop", Brand = "Apple", SellerId = sellerUser2.Id, MainImageUrl = "url_to_image" },
                        new Product { Name = "Dell XPS 13", Description = "Dell XPS 13", Price = 1299, Type = "Laptop", Brand = "Dell", SellerId = sellerUser2.Id, MainImageUrl = "url_to_image" }
                    });

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
