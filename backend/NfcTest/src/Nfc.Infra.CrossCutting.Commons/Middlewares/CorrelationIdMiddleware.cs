using Microsoft.AspNetCore.Http;
using Nfc.Application.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace Nfc.Infra.CrossCutting.Commons.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICorrelationContext correlationContext)
        {
            var headerValue = context.Request.Headers[HeaderName].FirstOrDefault();
            Guid correlationId;

            if (!string.IsNullOrWhiteSpace(headerValue) && Guid.TryParse(headerValue, out var parsed))
            {
                correlationId = parsed;
            }
            else
            {
                correlationId = Guid.NewGuid();
            }

            context.Items[HeaderName] = correlationId;
            correlationContext.CorrelationId = correlationId;

            context.Response.Headers[HeaderName] = correlationId.ToString();

            var current = Activity.Current;
            if (current is not null)
            {
                current.SetTag("correlation.id", correlationId.ToString());
            }

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}