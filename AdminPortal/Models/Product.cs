namespace AdminPortal.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string MainImageUrl { get; set; }
        public ICollection<ProductImage> Images { get; set; }

        
    }
}
