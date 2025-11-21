using CoreEntity = Nfc.Domain.Core.Models;

namespace Nfc.Domain.Interfaces.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : CoreEntity.Entity
    {
        IQueryable<TEntity> GetAllQuery { get; }
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(long id, CancellationToken cancellationToken);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
