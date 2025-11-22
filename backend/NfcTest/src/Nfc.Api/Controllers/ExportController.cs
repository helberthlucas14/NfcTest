using FC.Codeflix.Catalog.Api.ApiModels.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nfc.Application.Export;
using Nfc.Application.Export.Interfaces;
using Nfc.Application.UseCases.Export.ExportNotaFiscal;
using Nfc.Application.UseCases.Export.GetExportStatusByJobId;

namespace Nfc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IExportFileStorage _storage;
        public ExportController(IMediator mediator, IExportFileStorage storage)
        {
            _mediator = mediator;
            _storage = storage;
        }

        [HttpPost()]
        public async Task<IActionResult> ExportAsync(
            [FromBody] ExportNotaFiscalCommand command,
            CancellationToken cancellationToken)
        {
            var jobId = await _mediator.Send(command, cancellationToken);
            return Accepted(new { jobId, correlationId = command.CorrelationId });
        }

        [HttpGet("status/{jobId}")]
        [ProducesResponseType(typeof(ApiResponseList<ExportStatus>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string jobId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetExportStatusByJobIdQuery() { JobIdQuery = jobId }, cancellationToken);
            return Ok(response);
        }

        [HttpGet("file/{jobId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFile(string jobId, CancellationToken cancellationToken)
        {
            var status = await _mediator.Send(new GetExportStatusByJobIdQuery() { JobIdQuery = jobId }, cancellationToken);
            var fileInfo = await _storage.OpenReadAsync(jobId, status.Type, cancellationToken);
            if (fileInfo is null)
                return NotFound();
            return File(fileInfo.ContentStream, fileInfo.ContentType, fileInfo.FileName);
        }
    }
}
