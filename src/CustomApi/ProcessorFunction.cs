using CustomApi.Infrastructure.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CustomApi
{
    public static class ProcessorFunction
    {
        [FunctionName("ProcessorFunction")]
        public static IActionResult RunGetProcessor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processor")]
            HttpRequest request)
        {
            return new OkObjectResult(new { SomeSortOfFilter.InvocationCount });
        }
    }
}
