public interface IUnitOfWork : IDisposable
{
    IProductRepository ProductRepository { get; }
    Task<int> CommitAsync();
}