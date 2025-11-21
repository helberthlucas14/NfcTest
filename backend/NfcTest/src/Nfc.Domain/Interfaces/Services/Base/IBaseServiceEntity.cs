using EntityCore = Nfc.Domain.Core.Models;

namespace Nfc.Domain.Interfaces.Services.Base
{
    public interface IBaseServiceEntity<TEntity> : IBaseService where TEntity : EntityCore.Entity
    {
        Task<TEntity> RegisterAsync(TEntity entity, CancellationToken cancellationToken);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        IQueryable<TEntity> GetAllQuery { get; }
        Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(long id, CancellationToken cancellationToken);
        Task DeleteByIdAsync(long id, CancellationToken cancellationToken);
    }
}
