namespace EcommerceApi.Models.Dto.Products.Request
{
    public class ProductRequestDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
