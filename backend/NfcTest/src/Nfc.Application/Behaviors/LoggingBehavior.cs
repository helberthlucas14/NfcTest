
using MediatR;
using Nfc.Application.Logging;

namespace Nfc.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IApplicationLogging _appLogging;
        private readonly ICorrelationContext _context;

        public LoggingBehavior(
            IApplicationLogging appLogging,
            ICorrelationContext context)
        {
            _appLogging = appLogging;
            _context = context;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_context.CorrelationId == Guid.Empty)
                _context.CorrelationId = Guid.NewGuid();

            var correlationId = _context.CorrelationId;

            var jobId = request?.GetType().GetProperty("JobId")?.GetValue(request)?.ToString();

            var operation = typeof(TRequest).Name;
            var start = DateTime.UtcNow;

            _appLogging.LogStarted(correlationId, operation, jobId);

            try
            {
                var response = await next();

                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _appLogging.LogCompleted(correlationId, operation, duration, jobId);

                return response;
            }
            catch (Exception ex)
            {
                var duration = (DateTime.UtcNow - start).TotalMilliseconds;
                _appLogging.LogFailure(correlationId, operation, duration, ex, jobId);

                throw;
            }
        }
    }
}
