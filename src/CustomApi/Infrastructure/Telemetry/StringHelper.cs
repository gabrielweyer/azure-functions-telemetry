using System;

namespace CustomApi.Infrastructure.Telemetry
{
    public static class StringHelper
    {
        public static bool IsSame(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }
}