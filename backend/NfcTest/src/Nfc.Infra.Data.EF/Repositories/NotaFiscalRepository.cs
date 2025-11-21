using FC.Codeflix.Catalog.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Nfc.Domain.Entity;
using Nfc.Domain.Interfaces.Repositories;

namespace Nfc.Infra.Data.EF.Repositories
{
    public class NotaFiscalRepository : INotaFiscalRepository
    {

        private readonly NfcDbContext _context;
        private DbSet<NotaFiscal> _notas => _context.Set<NotaFiscal>();
        public NotaFiscalRepository(NfcDbContext context) => _context = context;

        public IQueryable<NotaFiscal> GetAllQuery => _notas
            .Include(n => n.Items);

        public async Task<NotaFiscal> AddAsync(NotaFiscal entity, CancellationToken cancellationToken)
        {
            await _notas.AddAsync(entity, cancellationToken);
            return entity;
        }

        public Task<bool> ExistsAsync(long id, CancellationToken cancellationToken) =>
            _notas.AnyAsync(x => x.Id == id, cancellationToken);

        public async Task<NotaFiscal> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var notafiscal = await _notas
                .Include(n => n.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            NotFoundException.ThrowIfNull(notafiscal, $"NotaFiscal '{id}' not found.");

            return notafiscal!;
        }

        public Task UpdateAsync(NotaFiscal entity, CancellationToken _)
                 => Task.FromResult(_notas.Update(entity));

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
