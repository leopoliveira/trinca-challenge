using System.Collections.Generic;
using System.Linq;

namespace Domain.Extensions
{
    public static class ObjectExtensions
    {
        public static bool HasValue(this object value)
        {
            return value != null;
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool HasValue<T>(this IList<T> value)
        {
            return value?.Any() == true;
        }
    }
}
