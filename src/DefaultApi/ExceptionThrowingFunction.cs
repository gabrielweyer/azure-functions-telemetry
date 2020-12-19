using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace DefaultApi
{
    public static class ExceptionThrowingFunction
    {
        [FunctionName("ExceptionThrowingFunction")]
        public static IActionResult RunGetException(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "exception")]
            HttpRequest request)
        {
            throw new InvalidOperationException("The only goal of this function is to throw an Exception.");
        }
    }
}
