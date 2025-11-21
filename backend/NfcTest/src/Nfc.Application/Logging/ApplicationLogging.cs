using Microsoft.Extensions.Logging;

namespace Nfc.Application.Logging
{
    public class ApplicationLogging : IApplicationLogging
    {
        private readonly ILogger<ApplicationLogging> _logger;
        public ApplicationLogging(ILogger<ApplicationLogging> logger) => _logger = logger;

        public void LogStarted(Guid correlationId, string operation, string? jobId = null)
        {
            _logger.LogInformation("Started {Operation} - {@LogObject}", operation, new
            {
                correlationId,
                jobId,
                operation,
                status = "Started"
            });
        }

        public void LogCompleted(Guid correlationId, string operation, double durationMs, string? jobId = null)
        {
            _logger.LogInformation("Completed {Operation} - {@LogObject}", operation, new
            {
                correlationId,
                jobId,
                operation,
                status = "Completed",
                durationMs
            });
        }

        public void LogFailure(Guid correlationId, string operation, double durationMs, Exception ex, string? jobId = null)
        {
            _logger.LogError(ex, "Fail {Operation} - {@LogObject}", operation, new
            {
                correlationId,
                jobId,
                operation,
                status = "Fail",
                durationMs
            });
        }
    }
}
