
public interface IProductService{
    Task Create(Product product);
    Task<IEnumerable<Product>> GetAsync();
    Task<Product> GetByIdAsync(Guid Id);
    Task Update(Product product);
    Task Delete(Product product);
}