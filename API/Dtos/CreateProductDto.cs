namespace API.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string MainImageUrl { get; set; }
    }
}
