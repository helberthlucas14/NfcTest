//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Nfc.Application.Logging;
//using System.Diagnostics;

//namespace Nfc.Infra.CrossCutting.Commons.Middlewares
//{
//    public class CorrelationIdMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly IApplicationLogging _appLogger;
//        private const string CorrelationHeader = "X-Correlation-ID";

//        public CorrelationIdMiddleware(RequestDelegate next, IApplicationLogging appLogger)
//        {
//            _next = next;
//            _appLogger = appLogger;
//        }
//        public async Task InvokeAsync(HttpContext context)
//        {
//            var correlationId = GetOrCreateCorrelationId(context);
//            context.Items[CorrelationHeader] = correlationId;

//            var stopwatch = Stopwatch.StartNew();

//            _appLogger.LogStarted(Guid.Parse(correlationId), "HTTP " + context.Request.Method + " " + context.Request.Path);

//            try
//            {
//                await _next(context);
//            }
//            finally
//            {
//                stopwatch.Stop();
//                _appLogger.LogCompleted(Guid.Parse(correlationId),
//                    "HTTP " + context.Request.Method + " " + context.Request.Path,
//                    stopwatch.ElapsedMilliseconds);
//            }
//        }

//        private string GetOrCreateCorrelationId(HttpContext context)
//        {
//            if (context.Request.Headers.TryGetValue(CorrelationHeader, out var headerValue))
//            {
//                if (Guid.TryParse(headerValue, out var parsedGuid))
//                {
//                    if (parsedGuid != Guid.Empty)
//                        return parsedGuid.ToString();
//                }
//            }

//            var newCorrelationId = Guid.NewGuid();
//            context.Request.Headers[CorrelationHeader] = newCorrelationId.ToString();
//            return newCorrelationId.ToString();
//        }
//    }
//}
