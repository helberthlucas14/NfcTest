using System.Diagnostics;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Prometheus;

namespace Nfc.Infra.Observability
{
    public static class HangfireMetrics
    {
        private static readonly Counter JobsFailedTotal = Metrics.CreateCounter("nfc_jobs_failed_total", "Jobs falhados", new CounterConfiguration
        {
            LabelNames = new[] { "job_type", "exception_type" }
        });
        private static readonly Counter JobsSucceededTotal = Metrics.CreateCounter("nfc_jobs_succeeded_total", "Jobs com sucesso", new CounterConfiguration
        {
            LabelNames = new[] { "job_type" }
        });
        private static readonly Histogram JobDurationMs = Metrics.CreateHistogram("nfc_job_duration_ms", "Duração do job em ms", new HistogramConfiguration
        {
            LabelNames = new[] { "job_type" },
            Buckets = Histogram.ExponentialBuckets(10, 2, 12)
        });
        public static void IncFailed(string jobType, string exceptionType) => JobsFailedTotal.WithLabels(jobType, exceptionType).Inc();
        public static void IncSucceeded(string jobType) => JobsSucceededTotal.WithLabels(jobType).Inc();
        public static void ObserveDuration(double ms, string jobType) => JobDurationMs.WithLabels(jobType).Observe(ms);
    }

    public class HangfireMetricsFilter : JobFilterAttribute, IServerFilter, IApplyStateFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
            filterContext.Items["__nfc_start"] = Stopwatch.GetTimestamp();
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Items.TryGetValue("__nfc_start", out var startObj) && startObj is long startTs)
            {
                var elapsed = (Stopwatch.GetTimestamp() - startTs) * 1000.0 / Stopwatch.Frequency;
                var jobType = filterContext.BackgroundJob?.Job?.Type?.FullName ?? "unknown";
                HangfireMetrics.ObserveDuration(elapsed, jobType);
            }
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var jobType = context.BackgroundJob?.Job?.Type?.FullName ?? "unknown";
            if (context.NewState is FailedState failed)
            {
                var exType = failed.Exception?.GetType().FullName ?? "unknown";
                HangfireMetrics.IncFailed(jobType, exType);
            }
            else if (context.NewState is SucceededState)
            {
                HangfireMetrics.IncSucceeded(jobType);
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }
    }
}