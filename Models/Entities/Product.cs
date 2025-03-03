using System.ComponentModel.DataAnnotations;

public class Product
{
    public Guid Id { get; private set; }
    [Required(ErrorMessage="Name is Required")]
    public string Name { get; private set; }

    public string? Description { get; private set; }
    [Required(ErrorMessage = "Price is Required")]
    public double Price { get; private set; }
    public string? ImageUrl { get; private set; }

    // Construtor privado para Entity Framework
    private Product() { }

    // Construtor principal
    public Product(string name, string? description, double price, string? imageUrl)
    {
        Id = Guid.NewGuid(); 
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetImage(imageUrl);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new NullReferenceException("O nome do produto não pode ser vazio.");
        Name = name;
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetPrice(double price)
    {
        if (price < 0)
            throw new ArgumentException("The price must be higher than zero.");
        
        Price = price;
    }

    public void SetImage(string? url)
    {
        ImageUrl = url;
    }

    // Método para atualizar várias propriedades de uma vez
    public void Update(string name, string description, double price, string imageUrl)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetImage(imageUrl);
    }
}
