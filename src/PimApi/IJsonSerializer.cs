using System;
using System.IO;
using System.Threading.Tasks;

namespace PimApi
{
    /// <summary>
    /// Allows for choice of JSON serializer
    /// </summary>
    public interface IJsonSerializer
    {
        string Serialize(object? data, Type? type);

        Task<TData?> DeserializeAsync<TData>(Stream data);

        /// <summary>
        /// IMPORTANT: Only use for demo purposes, not intended for production use. Strings can consume large amounts of memory cause performance issues
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        TData? Deserialize<TData>(string data);
    }
}
