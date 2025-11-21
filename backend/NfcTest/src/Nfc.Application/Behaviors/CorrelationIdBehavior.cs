using MediatR;
using Microsoft.AspNetCore.Http;
using Nfc.Application.UseCases.Base;

namespace Nfc.Application.Behaviors
{
    public class CorrelationIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : CommandRequestBase<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null &&
                context.Items.TryGetValue("X-Correlation-ID", out var correlationObj) &&
                Guid.TryParse(correlationObj?.ToString(), out var correlationId))
            {
                request.CorrelationId = correlationId;
            }

            return await next();
        }
    }
}
