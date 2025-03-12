
public interface IProductService{
    Task Create(Product product);
    Task<IEnumerable<Product>> GetAsync(string? txt);
    Task<IEnumerable<Product>> GetByCategoryAsync(string? category);
    Task<Product> GetByIdAsync(Guid Id);
    Task Update(Product product);
    Task Delete(Product product);
}