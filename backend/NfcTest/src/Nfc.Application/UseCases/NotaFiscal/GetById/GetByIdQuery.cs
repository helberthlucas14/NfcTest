using Nfc.Application.UseCases.Base;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Application.UseCases.NotaFiscal.CriarNotaFiscal;

namespace Nfc.Application.UseCases.NotaFiscal.GetById
{
    public class GetByIdQuery : CommandRequestBase<NotaFiscalResponse>
    {
        public long Id { get; set; }
    }
}
