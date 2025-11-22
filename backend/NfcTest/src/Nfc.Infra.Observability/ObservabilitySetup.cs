using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Prometheus;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using Serilog;
using Serilog.Exceptions;

namespace Nfc.Infra.Observability
{
    public static class ObservabilitySetup
    {
        public static readonly ActivitySource Activity = new("Nfc.Backend");
        public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
        {
            GlobalJobFilters.Filters.Add(new HangfireMetricsFilter());
            var serviceName = configuration["Observability:ServiceName"] ?? "Nfc.Api";
            var otlpEndpoint = configuration["Observability:OtlpEndpoint"] ?? "http://localhost:4318";

            services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName: serviceName))
                .WithTracing(tracer =>
                {
                    tracer
                        .SetSampler(new AlwaysOnSampler())
                        .AddSource(Activity.Name)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(exporter =>
                        {
                            exporter.Endpoint = new Uri(otlpEndpoint);
                            exporter.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        });
                });
            return services;
        }

        public static void ConfigureSerilog(WebApplicationBuilder builder)
        {
            var seqUrl = builder.Configuration["Observability:SeqUrl"] ?? "http://localhost:5341";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Hangfire", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                .WriteTo.Seq(seqUrl)
                .CreateLogger();
            builder.Host.UseSerilog();
        }

        public static void UseObservability(IApplicationBuilder app)
        {
            app.UseMetricServer();
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.000} ms";
                options.EnrichDiagnosticContext = (diag, http) =>
                {
                    diag.Set("CorrelationId", http.Items.TryGetValue("X-Correlation-ID", out var c) ? c?.ToString() : null);
                    diag.Set("ClientIP", http.Connection.RemoteIpAddress?.ToString());
                    diag.Set("UserAgent", http.Request.Headers["User-Agent"].ToString());
                    diag.Set("TraceId", http.TraceIdentifier);
                };
            });
        }
    }
}