using Microsoft.AspNetCore.Mvc;
using Nfc.Application.Export;

namespace Nfc.Api.Controllers
{
    [ApiController]
    [Route("api/export/status")]
    public class ExportStatusController : ControllerBase
    {
        private readonly IExportStatusRepository _repository;
        public ExportStatusController(IExportStatusRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{jobId}")]
        public async Task<IActionResult> Get(string jobId, CancellationToken cancellationToken)
        {
            var status = await _repository.GetAsync(jobId, cancellationToken);
            if (status is null) return NotFound();
            return Ok(status);
        }
    }
}