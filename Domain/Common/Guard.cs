using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{ 
    public static class Guard
    {
        public static string NotNullOrWhiteSpace(string? value, string paramName, int maxLen = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} is required.", paramName);
    
            var trimmed = value.Trim();
            if (trimmed.Length > maxLen)
                throw new ArgumentException($"{paramName} length must be <= {maxLen}.", paramName);
    
            return trimmed;
        }
    
        public static string? TrimToNull(string? value, int maxLen = int.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var trimmed = value.Trim();
            if (trimmed.Length > maxLen)
                throw new ArgumentException($"Value length must be <= {maxLen}.", nameof(value));
            return trimmed;
        }
    }
}
