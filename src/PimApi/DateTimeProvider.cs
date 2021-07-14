using System;

namespace PimApi
{
    public class DateTimeProvider
    {
        public readonly static DateTimeProvider Default = new();

        public virtual DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
    }
}
