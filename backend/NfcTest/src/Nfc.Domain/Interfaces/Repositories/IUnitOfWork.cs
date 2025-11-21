namespace Nfc.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public Task CommitAsync(CancellationToken cancellationToken);
    }


}
