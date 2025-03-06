
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    public ProductService(IProductRepository repository){
        _repository = repository;
    }

    public async Task Create(Product product)
    {
        if(product == null){
        throw new ArgumentNullException("Product not found");
        }
        await _repository.Create(product);
    }

    public async Task Delete(Product product)
    {
        if (product == null){
            throw new ArgumentNullException("Product not found");
        }
        await _repository.Delete(product);
    }

    public async Task<IEnumerable<Product>> GetAsync()
    {
        return await _repository.GetAsync();
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public  async Task Update(Product product)
    {
        if( product == null){
            throw new NullReferenceException();
        }
        await _repository.Update(product);
    }
}