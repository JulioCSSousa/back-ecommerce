
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

    public async Task<IQueryable<Product>> GetAsync(string? txt)
    {
        var query = String.IsNullOrEmpty(txt) ? _context.Products.AsQueryable() : 
        _context.Products.Where(p => p.Name.Contains(txt) 
        || p.Description.Contains(txt));
        return query;
    }
    public async Task<IQueryable<Product>> GetByCategory(string? category){
        return _context.Products.Where(p => p.Category.ToLower() == category.ToLower());
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