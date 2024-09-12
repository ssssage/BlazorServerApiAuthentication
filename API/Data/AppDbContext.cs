using API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace API.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Case-insensitive index on DisplayName
            builder.Entity<User>()
.                   HasIndex(u => u.DisplayName)
.                   IsUnique(); // Or use modelBuilder.HasIndex(u => u.DisplayName).IsUnique();
        }
    }

}

//dotnet ef database update -p API -s API -c AppDbContext
//dotnet-ef migrations add "TheApiDb" -p API -s API -c AppDbContext -o Data/Migrations
//dotnet ef migrations remove 
//dotnet ef database drop
//dotnet ef database update
//dotnet-ef migrations add "TheApiDb" -o Data/Migrations
