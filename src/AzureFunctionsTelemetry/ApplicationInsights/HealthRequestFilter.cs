namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights
{
    public class HealthRequestFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;
        private readonly string _healthCheckFunctionName;

        public HealthRequestFilter(ITelemetryProcessor next, string healthCheckFunctionName)
        {
            _next = next;
            _healthCheckFunctionName = healthCheckFunctionName;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry request &&
                request.ResponseCode == "200" &&
                StringHelper.IsSame(_healthCheckFunctionName, request.Name))
            {
                return;
            }

            _next.Process(item);
        }
    }
}