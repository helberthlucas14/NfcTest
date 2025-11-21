using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nfc.Application.Export;
using Nfc.Application.UseCases.ExportNotaFiscal;
using Nfc.Infra.HangFire.Jobs;

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
    }
}
