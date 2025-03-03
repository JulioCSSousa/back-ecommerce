
public interface IProductService{
    Task Create(Product product);
    Task<IEnumerable<Product>> GetAsync();
    Task<Product> GetByIdAsync(Guid guid);
    Task Update(Product product);
    Task Delete(Product product);
}