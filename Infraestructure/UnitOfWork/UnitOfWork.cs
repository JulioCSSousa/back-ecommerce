public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IProductRepository _product;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProductRepository ProductRepository => _product ??= new ProductRepository(_context);

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
