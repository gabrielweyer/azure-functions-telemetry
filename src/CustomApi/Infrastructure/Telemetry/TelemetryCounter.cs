using System.Threading;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace CustomApi.Infrastructure.Telemetry
{
    public class TelemetryCounter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public long AvailabilityTelemetryCount;
        public long DependencyTelemetryCount;
        public long EventTelemetryCount;
        public long ExceptionTelemetryCount;
        public long MetricTelemetryCount;
        public long PageViewPerformanceTelemetryCount;
        public long PageViewTelemetryCount;
        public long PerformanceCounterTelemetryCount;
        public long RequestTelemetryCount;
        public long SessionStateTelemetryCount;
        public long TraceTelemetryCount;
        public long OtherTelemetryCount;

        public TelemetryCounter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            switch (item)
            {
                case AvailabilityTelemetry _:
                    Interlocked.Increment(ref AvailabilityTelemetryCount);
                    break;
                case DependencyTelemetry _:
                    Interlocked.Increment(ref DependencyTelemetryCount);
                    break;
                case EventTelemetry _:
                    Interlocked.Increment(ref EventTelemetryCount);
                    break;
                case ExceptionTelemetry _:
                    Interlocked.Increment(ref ExceptionTelemetryCount);
                    break;
                case MetricTelemetry _:
                    Interlocked.Increment(ref MetricTelemetryCount);
                    break;
                case PageViewPerformanceTelemetry _:
                    Interlocked.Increment(ref PageViewPerformanceTelemetryCount);
                    break;
                case PageViewTelemetry _:
                    Interlocked.Increment(ref PageViewTelemetryCount);
                    break;
                case PerformanceCounterTelemetry _:
                    Interlocked.Increment(ref PerformanceCounterTelemetryCount);
                    break;
                case RequestTelemetry _:
                    Interlocked.Increment(ref RequestTelemetryCount);
                    break;
                case SessionStateTelemetry _:
                    Interlocked.Increment(ref SessionStateTelemetryCount);
                    break;
                case TraceTelemetry _:
                    Interlocked.Increment(ref TraceTelemetryCount);
                    break;
                default:
                    Interlocked.Increment(ref OtherTelemetryCount);
                    break;
            }

            _next.Process(item);
        }
    }
}