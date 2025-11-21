using MediatR;
using Microsoft.Extensions.Logging;
using Nfc.Application.Logging;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Domain.Interfaces.Repositories;
using Nfc.Domain.Interfaces.Services;
using DomainEntity = Nfc.Domain.Entity;

namespace Nfc.Application.UseCases.NotaFiscal.CriarNotaFiscal
{
    public class CriarNotaFiscalCommandHandler :
        IRequestHandler<CriarNotaFiscalCommand, NotaFiscalResponse>
    {
        private readonly INotaFiscalService _service;
        private readonly IUnitOfWork _uof;

        public CriarNotaFiscalCommandHandler(
            INotaFiscalService service,
            IUnitOfWork uof)
        {
            _service = service;
            _uof = uof;
        }

        public async Task<NotaFiscalResponse> Handle(CriarNotaFiscalCommand request, CancellationToken cancellationToken)
        {
            var notaFiscal = new DomainEntity.NotaFiscal(request.Emissor, request.DataEmissao);
           
            foreach (var item in request.Items)
                notaFiscal.AdicionarItem(new DomainEntity.Item(
                    notaFiscal.Id,
                    item.Descricao,
                    item.Valor)
             );

            var entity = await _service.RegisterAsync(notaFiscal, cancellationToken);
            await _uof.CommitAsync(cancellationToken);
            return NotaFiscalResponse.FromMember(entity);
        }
    }
}