using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public ProductController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.Images).ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> CreateProduct(CreateProductDto productDto)
        {
            var user = await _userManager.GetUserAsync(User);

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Type = productDto.Type,
                Brand = productDto.Brand,
                SellerId = user.Id,
                MainImageUrl = productDto.MainImageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> UpdateProduct(int id, UpdateProductDto productDto)
        {
            var user = await _userManager.GetUserAsync(User);
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.SellerId != user.Id)
            {
                return NotFound("Product not found or you are not authorized to update this product");
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Type = productDto.Type;
            product.Brand = productDto.Brand;
            product.MainImageUrl = productDto.MainImageUrl;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var product = await _context.Products.FindAsync(id);

            if (product == null || (product.SellerId != user.Id && !User.IsInRole("Admin")))
            {
                return NotFound("Product not found or you are not authorized to delete this product");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UploadProductImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Validate file extension
            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Validate MIME type
            var validMimeTypes = new[] { "image/jpeg", "image/png" };
            if (!validExtensions.Contains(ext) || !validMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return BadRequest("Invalid file type. Only .jpg and .png files are allowed.");
            }


            var uploadsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ProductImages");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var imageName = $"{file.FileName}";
            var path = Path.Combine("Content/images/products", imageName);


            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            // Save file path in the database as part of the product image
            var image = new ProductImage { ImageUrl = path };
            // Save image to the DB here
            return Ok(new { path = $"{image}" });
           
        }


    }

}
