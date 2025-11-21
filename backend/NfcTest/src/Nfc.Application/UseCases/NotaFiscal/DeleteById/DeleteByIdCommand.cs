using MediatR;
using Nfc.Application.UseCases.Base;

namespace Nfc.Application.UseCases.NotaFiscal.DeleteById
{
    public class DeleteByIdCommand : CommandRequestBase<Unit>
    {
        public long Id { get; set; }
    }
}
