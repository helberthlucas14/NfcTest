using MediatR;
using Nfc.Application.UseCases.Base;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Domain.Interfaces.Services;

namespace Nfc.Application.UseCases.NotaFiscal.GetAll
{
    public class GetAllQuery : CommandRequestBase<PagedList<NotaFiscalResponse>>
    {
        public NotaFiscalQueryStringParameters parameters;

        public GetAllQuery(NotaFiscalQueryStringParameters parameters)
        {
            this.parameters = parameters;
        }
    }

    public class GetAllQueryHandler : IRequestHandler<GetAllQuery, PagedList<NotaFiscalResponse>>
    {
        private readonly INotaFiscalService _service;

        public GetAllQueryHandler(INotaFiscalService service) => _service = service;

        public async Task<PagedList<NotaFiscalResponse>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            var result = await _service.GetAllQueryAsync(request.parameters, cancellationToken);

            var mappedResult = result.Select(NotaFiscalResponse.FromMember).ToList();

            return await Task.FromResult(new PagedList<NotaFiscalResponse>(
                mappedResult,
                result.TotalRecords,
                request.parameters.PageNumber,
                request.parameters.PageSize));
        }
    }
}
