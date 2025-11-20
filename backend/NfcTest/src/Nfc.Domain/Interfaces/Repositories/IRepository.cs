using CoreEntity = Nfc.Domain.Core.Models;

namespace Nfc.Domain.Interfaces.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : CoreEntity.Entity
    {
        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity> GetByIdAsync(Guid id);

        IQueryable<TEntity> GetAllQuery { get; }

        Task<bool> ExistsAsync(Guid id);

        public Task<bool> UpdateAsync(TEntity entity);
    }
}
