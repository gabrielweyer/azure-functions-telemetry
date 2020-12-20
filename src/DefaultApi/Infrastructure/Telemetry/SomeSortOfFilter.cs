using System.Threading;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace DefaultApi.Infrastructure.Telemetry
{
    public class SomeSortOfFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        public static int InvocationCount;

        public SomeSortOfFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            Interlocked.Increment(ref InvocationCount);
            _next.Process(item);
        }
    }
}