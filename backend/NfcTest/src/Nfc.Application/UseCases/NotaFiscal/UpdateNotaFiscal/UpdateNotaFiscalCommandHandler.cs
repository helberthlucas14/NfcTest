using MediatR;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Domain.Interfaces.Repositories;
using Nfc.Domain.Interfaces.Services;

namespace Nfc.Application.UseCases.NotaFiscal.UpdateNotaFiscal
{
    public class UpdateNotaFiscalCommandHandler : IRequestHandler<UpdateNotaFiscalCommand, NotaFiscalResponse>
    {
        private readonly INotaFiscalService _service;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateNotaFiscalCommandHandler(INotaFiscalService service, IUnitOfWork unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        public async Task<NotaFiscalResponse> Handle(UpdateNotaFiscalCommand request, CancellationToken cancellationToken)
        {
            var entity = await _service.UpdateAsync(
                  request.Id,
                  request.Emissor,
                  request.DataEmissao,
                  request.Items.Select(i => new Domain.Entity.Item(request.Id, i.Descricao, i.Valor)).ToList(),
                  cancellationToken
                  );
            await _unitOfWork.CommitAsync(cancellationToken);
            return NotaFiscalResponse.FromMember(entity);
        }
    }
}
