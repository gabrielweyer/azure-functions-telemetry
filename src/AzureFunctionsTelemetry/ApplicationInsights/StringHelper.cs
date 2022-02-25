using System;

namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights
{
    internal static class StringHelper
    {
        public static bool IsSame(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}