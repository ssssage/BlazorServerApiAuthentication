using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public bool IsSeller { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
