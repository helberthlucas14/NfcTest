using MediatR;
using Nfc.Application.Logging;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Domain.Interfaces.Services;

namespace Nfc.Application.UseCases.NotaFiscal.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, NotaFiscalResponse>
    {
        private readonly INotaFiscalService _service;
        public GetByIdQueryHandler(INotaFiscalService service)
        {
            _service = service;
        }

        public async Task<NotaFiscalResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _service.GetByIdAsync(request.Id, cancellationToken);
            return NotaFiscalResponse.FromMember(entity);
        }
    }
}
