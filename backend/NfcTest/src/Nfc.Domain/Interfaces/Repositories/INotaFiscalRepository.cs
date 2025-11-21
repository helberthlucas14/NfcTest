using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Domain.Entity;

namespace Nfc.Domain.Interfaces.Repositories
{
    public interface INotaFiscalRepository
        : IRepository<NotaFiscal>
    {
        Task<PagedList<NotaFiscal>> GetAllAsync(NotaFiscalQueryStringParameters parameters,
                   CancellationToken cancellationToken);
    }
}
