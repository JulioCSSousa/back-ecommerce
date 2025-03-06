public interface IProductRepository{
    Task Create(Product product);
    Task<IQueryable<Product>> GetAsync();
    Task<Product> GetByIdAsync(Guid id);
    Task Update(Product product);
    Task Delete(Product  product);
}