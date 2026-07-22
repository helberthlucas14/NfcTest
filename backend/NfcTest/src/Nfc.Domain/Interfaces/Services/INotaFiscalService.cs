using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Domain.Entity;
using Nfc.Domain.Interfaces.Services.Base;

namespace Nfc.Domain.Interfaces.Services
{
    public interface INotaFiscalService : IBaseServiceEntity<NotaFiscal>
    {
       
        Task<PagedList<NotaFiscal>> GetAllQueryAsync(NotaFiscalQueryStringParameters parameters,
            CancellationToken cancellationToken);

        Task<IEnumerable<long>> GetExistingIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken);
        Task<IEnumerable<NotaFiscal>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken);

        Task<NotaFiscal> UpdateAsync(long id, string emissor, DateTime dataEmissao, IList<Item> items,
            CancellationToken cancellationToken);
    }
}
