using PimApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PimApi.JsonSerialization
{
    public class SystemTextJsonSerialzer : IJsonSerializer
    {
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SystemTextJsonSerialzer()
        {
            // adds SystemTextJsonODataConverter for all entity types
            var converterTypes = new Type[]
            {
                typeof(SystemTextJsonODataCollectionConverter<>),
                typeof(SystemTextJsonEntityConverter<>)
            };

            foreach (var type in this.GetEntityTypes())
            {
                foreach (var converterType in converterTypes)
                {
                    if (Activator.CreateInstance(converterType.MakeGenericType(type)) is not JsonConverter converter) { continue; }

                    jsonSerializerOptions.Converters.Add(converter);
                }
            }
        }

        public SystemTextJsonSerialzer(ICollection<JsonConverter> converters)
        {
            foreach (var converter in converters)
            {
                jsonSerializerOptions.Converters.Add(converter);
            }
        }

        public TData? Deserialize<TData>(string data) =>
            JsonSerializer.Deserialize<TData>(data, jsonSerializerOptions);

        public async Task<TData?> DeserializeAsync<TData>(Stream data) =>
            await JsonSerializer.DeserializeAsync<TData>(data, jsonSerializerOptions);

        public string Serialize(object? data, Type? type) => type is null
            ? JsonSerializer.Serialize(data)
            : JsonSerializer.Serialize(data, type);
    }
}