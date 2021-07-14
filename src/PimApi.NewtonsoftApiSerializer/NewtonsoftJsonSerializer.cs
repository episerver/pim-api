using Newtonsoft.Json;
using PimApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PimApi.JsonSerialization
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializer jsonSerializer;

        private readonly JsonSerializerSettings jsonSerializerOptions = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        public NewtonsoftJsonSerializer()
        {
            // adds NewtonsoftJsonODataResponseConverter for all entity types
            var converterTypes = new Type[]
            {
                typeof(NewtonsoftJsonJsonEntityConverter<>),
                typeof(NewtonsoftJsonODataResponseConverter<>)
            };

            foreach (var type in this.GetEntityTypes())
            {
                foreach (var converterType in converterTypes)
                {
                    if (Activator.CreateInstance(converterType.MakeGenericType(type)) is not JsonConverter converter) { continue; }

                    jsonSerializerOptions.Converters.Add(converter);
                }
            }

            jsonSerializer = JsonSerializer.Create(jsonSerializerOptions);
        }

        public NewtonsoftJsonSerializer(ICollection<JsonConverter> converters)
        {
            foreach (var converter in converters)
            {
                jsonSerializerOptions.Converters.Add(converter);
            }

            jsonSerializer = JsonSerializer.Create(jsonSerializerOptions);
        }

        public TData? Deserialize<TData>(string data) =>
            JsonConvert.DeserializeObject<TData>(data, jsonSerializerOptions);

        public async Task<TData?> DeserializeAsync<TData>(Stream data)
        {
            await Task.CompletedTask;
            using var sr = new StreamReader(data);
            using var reader = new JsonTextReader(sr);

            return jsonSerializer.Deserialize<TData>(reader);
        }

        public string Serialize(object? data, Type? type) => type is null
            ? JsonConvert.SerializeObject(data, jsonSerializerOptions)
            : JsonConvert.SerializeObject(data, type, jsonSerializerOptions);
    }
}