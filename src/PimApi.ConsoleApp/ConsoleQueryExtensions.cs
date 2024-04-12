using PimApi.ConsoleApp.Queries;
using PimApi.ConsoleApp.Renderers.Miscellaneous;

namespace PimApi.ConsoleApp;

internal static class ConsoleQueryExtensions
{
    private static readonly List<IApiResponseMessageRenderer> RendererChoices = new();

    internal static List<IApiResponseMessageRenderer> GetRenderChoices(
        this IQueryWithMessageRenderer _
    )
    {
        if (RendererChoices.Count > 0)
        {
            return RendererChoices;
        }

        var rendererType = typeof(UserChoiceRenderer)
            .Assembly.GetTypes()
            .Where(o =>
                typeof(IApiResponseMessageRenderer).IsAssignableFrom(o)
                && o.IsClass
                && !o.IsAbstract
            );

        foreach (var type in rendererType)
        {
            if (typeof(UserChoiceRenderer).IsAssignableFrom(type))
            {
                continue;
            }
            if (Activator.CreateInstance(type) is not IApiResponseMessageRenderer renderer)
            {
                continue;
            }

            if (renderer is EntityListRenderer)
            {
                RendererChoices.Insert(0, renderer);
                continue;
            }

            RendererChoices.Add(renderer);
        }

        return RendererChoices;
    }

    internal static Guid? GetIdValue(this IQueryWithEntityId query)
    {
        var id = query.Id ?? Program.ReadValue($"Please enter the ID:", Guid.Empty);

        return id == Guid.Empty ? null : id;
    }

    internal static Guid? GetParentIdValue(this IQueryWithParentId query)
    {
        var id = query.ParentId ?? Program.ReadValue<Guid?>($"Please enter the parent ID:", null);

        return id is null || id == Guid.Empty ? null : id;
    }

    internal static int? GetSkipValue(this IQueryWithTopSkip query)
    {
        var skip = query.Skip ?? Program.ReadValue($"Please enter number of records to skip:", 0);

        return skip > 0 ? skip : null;
    }

    internal static int? GetTopValue(this IQueryWithTopSkip query)
    {
        var top = query.Top ?? Program.ReadValue($"Please enter number of records to return:", 25);

        return top > 0 ? top : null;
    }

    internal static void WriteBaseEntityInfo(
        this BaseEntityDto entity,
        Action<string> messageWriter
    )
    {
        if (messageWriter is null)
        {
            throw new ArgumentNullException(nameof(messageWriter));
        }
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        messageWriter(string.Empty);
        messageWriter($"{nameof(BaseEntityDto.Id)} = {entity.Id}");
        messageWriter($"{nameof(BaseEntityDto.CreatedBy)} = {entity.CreatedBy}");
        messageWriter($"{nameof(BaseEntityDto.CreatedOn)} = {entity.CreatedOn:s}");
        messageWriter($"{nameof(BaseEntityDto.ModifiedBy)} = {entity.ModifiedBy}");
        messageWriter($"{nameof(BaseEntityDto.ModifiedOn)} = {entity.ModifiedOn:s}");
        messageWriter(string.Empty);
    }

    internal static void WriteTotalCount<TEntity>(
        this ODataResponseCollection<TEntity> response,
        Action<string> messageWriter
    )
        where TEntity : BaseEntityDtoWithEndpoint
    {
        if (response.Count is not null && response.Count > response.Value.Count)
        {
            messageWriter(
                $" Found: {response.Count.Value:N0} matching {response.Value.FirstOrDefault()?.EntityUrlBase ?? "records"}"
            );
        }
    }

    internal static void WriteTotalCount<TEntity>(
        this ODataResponse<ICollection<TEntity>> response,
        Action<string> messageWriter
    )
        where TEntity : BaseEntityDto
    {
        if (response.Value is null)
        {
            return;
        }

        if (response.Count is not null && response.Count < response.Value.Count)
        {
            messageWriter($" Found: {response.Count.Value:N0} matching records");
        }
    }

    internal static string DisplayValue(this DateTimeOffset dateTimeOffset) =>
        $"{dateTimeOffset:s}";

    internal static string DisplayValue(this DateTimeOffset? dateTimeOffset) =>
        dateTimeOffset is null ? "null" : $"{dateTimeOffset:s}";

    internal static string DisplayValue(this object? value)
    {
        if (value is null)
        {
            return "null";
        }

        if (value is string s && s.Length == 0)
        {
            return "empty string";
        }

        if (value is bool b)
        {
            return b ? "true" : "false";
        }

        if (value is ICollection<string> collection)
        {
            return string.Join(',', collection);
        }

        if (value is ICollection<Guid> ids)
        {
            return string.Join(',', ids);
        }

        return value?.ToString() ?? string.Empty;
    }
}
