using Nfc.Domain.Interfaces.Repositories;

namespace Nfc.Infra.Data.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NfcDbContext _context;

        public UnitOfWork(NfcDbContext context) => _context = context;

        public async Task CommitAsync(CancellationToken cancellationToken) =>
            await _context.SaveChangesAsync(cancellationToken);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _context.Dispose();
        }
    }
}
