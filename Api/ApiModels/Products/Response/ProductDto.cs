using System.ComponentModel.DataAnnotations;

public class ProductDto{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }

}