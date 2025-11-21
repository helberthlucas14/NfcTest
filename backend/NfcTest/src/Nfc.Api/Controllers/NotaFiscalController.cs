using FC.Codeflix.Catalog.Api.ApiModels.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nfc.Application.UseCases.NotaFiscal.Common;
using Nfc.Application.UseCases.NotaFiscal.CriarNotaFiscal;
using Nfc.Application.UseCases.NotaFiscal.GetById;

namespace Nfc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotaFiscalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotaFiscalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(ApiResponse<NotaFiscalResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CriarNotaFiscal([FromBody] CriarNotaFiscalCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(CriarNotaFiscal),
                new { id = Convert.ToInt64(result.Id) },
                new ApiResponse<NotaFiscalResponse>(result));
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ApiResponse<NotaFiscalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetByIdQuery() { Id = id }, cancellationToken);
            return Ok(new ApiResponse<NotaFiscalResponse>(result));
        }

    }
}
