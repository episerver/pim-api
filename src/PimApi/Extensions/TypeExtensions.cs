using System.Reflection;

namespace PimApi.Extensions;

public static class TypeExtensions
{
    private static readonly List<Type> entityTypes = new(50);

    private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> TypeProperties =
        new(50);

    public static bool CanBeODataResponse<TEntity>(this Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }
        if (!typeof(ODataResponse).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition()))
        {
            return false;
        }

        var genericType = typeToConvert.GetGenericArguments()[0];

        return typeof(TEntity).IsAssignableFrom(genericType)
            || typeof(ICollection<TEntity>).IsAssignableFrom(genericType);
    }

    public static bool CanAssignValue(this PropertyInfo? propertyInfo, Type t) =>
        propertyInfo is not null && t.IsAssignableFrom(propertyInfo.PropertyType);

    /// <summary>Gets all types deriving from <see cref="BaseEntityDto.cs"/> in the assembly where it exists</summary>
    public static List<Type> GetEntityTypes(this IJsonSerializer _)
    {
        if (entityTypes.Count > 0)
        {
            return entityTypes;
        }

        foreach (var type in typeof(BaseEntityDto).Assembly.GetTypes())
        {
            if (
                !typeof(BaseEntityDto).IsAssignableFrom(type)
                || type.IsAbstract
                || type.GetConstructors().All(o => o.GetParameters().Length != 0)
            )
            {
                continue;
            }

            entityTypes.Add(type);
        }

        return entityTypes;
    }

    /// <summary>Gets case insensitive dictionary of type properties for JSON converter</summary>
    public static Dictionary<string, PropertyInfo> GetTypeProperties(this Type t) =>
        TypeProperties.TryGetValue(t, out var properties)
            ? properties
            : (
                TypeProperties[t] = properties =
                    t.GetProperties().ToDictionary(o => o.Name, StringComparer.OrdinalIgnoreCase)
            );
}
