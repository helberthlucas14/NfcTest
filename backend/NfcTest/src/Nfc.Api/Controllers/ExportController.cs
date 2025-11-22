using FC.Codeflix.Catalog.Api.ApiModels.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nfc.Application.Export;
using Nfc.Application.UseCases.Export.ExportNotaFiscal;
using Nfc.Application.UseCases.Export.GetExportStatusByJobId;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Infra.HangFire.Jobs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Nfc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> ExportAsync(
            [FromBody] ExportNotaFiscalCommand command,
            CancellationToken cancellationToken)
        {
            var jobId = await _mediator.Send(command, cancellationToken);
            return Accepted(new { jobId });
        }

        [HttpGet("status/{jobId:guid}")]
        [ProducesResponseType(typeof(ApiResponseList<ExportStatus>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid jobId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetExportStatusByJobIdQuery() { JobIdQuery = jobId }, cancellationToken);
            return Ok(response);
        }
    }
}
