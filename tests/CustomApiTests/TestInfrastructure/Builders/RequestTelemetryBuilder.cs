using System;
using Microsoft.ApplicationInsights.DataContracts;

namespace CustomApiTests.TestInfrastructure.Builders
{
    public static class RequestTelemetryBuilder
    {
        public static RequestTelemetry AsServiceBus()
        {
            return new()
            {
                Success = true,
                Name = "ServiceBusFunction",
                ResponseCode = "0",
                Url = null
            };
        }

        public static RequestTelemetry AsHttp()
        {
            return new()
            {
                Success = true,
                Name = "HttpFunction",
                ResponseCode = "200",
                Url = new Uri("http://localhost:7071/api/http")
            };
        }
    }
}