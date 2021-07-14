using PimApi.Entities;
using System;

namespace PimApi.Extensions
{
    public static class BaseEntityExtensions
    {
        public static int GetCount(
            this BaseEntityDto entity,
            ref int? count,
            string propertyName)
        {
            if (count is not null) { return count.Value; }

            count = entity.PropertyBag.TryGetValue($"{propertyName}{Defaults.oDataCount}", out var p)
                ? Convert.ToInt32(p)
                : 0;

            return count.Value;
        }

        public static bool HasCount(
            this BaseEntityDto entity,
            string propertyName) =>
            entity.PropertyBag.ContainsKey($"{propertyName}{Defaults.oDataCount}");
    }
}
