using DefaultV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace DefaultV4InProcessFunction.Functions.Processor;

public static class ProcessorFunction
{
    [FunctionName(nameof(ProcessorFunction))]
    public static IActionResult RunGetProcessor(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processor")]
        HttpRequest request)
    {
        return new OkObjectResult(new { SomeSortOfFilter.InvocationCount });
    }
}