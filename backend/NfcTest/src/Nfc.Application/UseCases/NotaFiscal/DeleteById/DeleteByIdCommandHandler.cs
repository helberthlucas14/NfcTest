using MediatR;
using Nfc.Domain.Interfaces.Repositories;
using Nfc.Domain.Interfaces.Services;

namespace Nfc.Application.UseCases.NotaFiscal.DeleteById
{
    public class DeleteByIdCommandHandler : IRequestHandler<DeleteByIdCommand, Unit>
    {
        private readonly INotaFiscalService _service;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteByIdCommandHandler(
            INotaFiscalService service,
            IUnitOfWork unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteByIdCommand request, CancellationToken cancellationToken)
        {
            await _service.DeleteByIdAsync(request.Id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
