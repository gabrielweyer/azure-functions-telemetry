using System;

namespace Custom.FunctionsTelemetry.ApplicationInsights
{
    internal static class StringHelper
    {
        public static bool IsSame(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}