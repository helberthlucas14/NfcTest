using MediatR;
using Nfc.Application.Export;

namespace Nfc.Application.UseCases.Export.GetExportStatusByJobId
{
    public class GetExportStatusByJobIdQueryHandler : IRequestHandler<GetExportStatusByJobIdQuery, ExportStatus>
    {
        private readonly IExportStatusRepository _repository;

        public GetExportStatusByJobIdQueryHandler(IExportStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExportStatus> Handle(GetExportStatusByJobIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAsync(request.JobIdQuery, cancellationToken);
            return result;
        }
    }
}
