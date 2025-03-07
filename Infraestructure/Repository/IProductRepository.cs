public interface IProductRepository{
    Task Create(Product product);
    Task<IQueryable<Product>> GetAsync(string? txt);
    Task<IQueryable<Product>> GetByCategory(string? category);
    Task<Product> GetByIdAsync(Guid id);
    Task Update(Product product);
    Task Delete(Product  product);
}