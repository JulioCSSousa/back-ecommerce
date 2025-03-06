
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context){
        _context = context;
    }

    public async Task Create(Product product)
    {
        if(product == null){
        throw new ArgumentNullException("Product not found");
        }
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Product product)
    {
        if (product == null){
            throw new ArgumentNullException("Product not found");
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<Product>> GetAsync()
    {
        return _context.Products.AsQueryable();
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
    }

    public  async Task Update(Product product)
    {
        _context.Update(product);
        await _context.SaveChangesAsync();
    }
}