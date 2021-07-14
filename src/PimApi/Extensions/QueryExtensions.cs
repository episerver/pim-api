using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PimApi.Extensions
{
    public static class QueryExtensions
    {
        public static T GetValueOrFallback<T>(this string? s, T fallback)
        {
            if (s is null || s.Length == 0)
            {
                return fallback;
            }

            if (fallback is string && s is object ss) { return (T)ss; }

            if (fallback is Guid || typeof(Guid?) == typeof(T))
            {
                if (!Guid.TryParse(s.Trim(), out var guid)) { return fallback; }

                // boxes but unavoidable
                return (T)(object)guid;
            }

            try { return (T)Convert.ChangeType(s.Trim(), typeof(T)); }
            catch { return fallback; }
        }

        public static string GetQueryFormat(this DateTime dateTime) =>
            dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

        public static string GetPreviousDate(this DateTimeProvider dateTimeProvider, int previousDays) =>
            dateTimeProvider.GetUtcNow()
                .Date
                .AddDays(-1 * Math.Abs(previousDays))
                .GetQueryFormat();

        public static bool IsYes(this string? s) => s?.Length > 0 && (s[0].Equals('y') || s[0].Equals('Y'));

        public static string GetValueToSearch(this string? valueToSearch) =>
           valueToSearch is null || valueToSearch == "null"
                ? "null"
                : $"'{valueToSearch}'";

        public static QueryDescriptor GetQueryDescriptor(this IQuery query)
        {
            var queryAttribute = GetDisplayAttribute(query);
            var queryType = query.GetType();

            return new QueryDescriptor
            {
                QueryNumber = queryAttribute?.Order ?? 0,
                DisplayName = queryAttribute?.Name ?? queryType.Name,
                Description = queryAttribute?.Description ?? "n/a",
                Group = queryAttribute?.GroupName ?? "Other",
                Query = query
            };
        }

        private static DisplayAttribute? GetDisplayAttribute(IQuery query) =>
            query.GetType().GetCustomAttribute<DisplayAttribute>();
    }
}
