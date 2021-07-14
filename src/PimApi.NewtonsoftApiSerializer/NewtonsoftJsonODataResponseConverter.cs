using Newtonsoft.Json;
using PimApi.Entities;
using PimApi.Extensions;
using System;
using System.Collections.Generic;
using static PimApi.Defaults;

namespace PimApi.JsonSerialization
{
    public class NewtonsoftJsonODataResponseConverter<TEntity> : JsonConverter
        where TEntity : BaseEntityDto
    {
        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanConvert(Type typeToConvert) => typeToConvert.CanBeODataResponse<TEntity>();

        public override object ReadJson(JsonReader reader, Type typeToConvert, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) { return null!; }

            var isCollection =
                typeof(ODataResponse<ICollection<TEntity>>).IsAssignableFrom(typeToConvert)
                || typeof(ODataResponseCollection<TEntity>).IsAssignableFrom(typeToConvert);
            ODataResponse response = isCollection
                ? new ODataResponseCollection<TEntity>()
                : new ODataResponse<TEntity>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject) { break; }

                var propertyName = reader.Value;
                switch (propertyName)
                {
                    case oDataContext:
                        response.Context = reader.ReadAsString();
                        break;

                    case oDataCount:
                        response.Count = reader.ReadAsInt32();
                        break;

                    case oDataNextLink:
                        response.NextLink = reader.ReadAsString();
                        break;

                    case oDataValue:
                        reader.Read();
                        response.Value = isCollection
                            ? serializer.Deserialize<ICollection<TEntity>>(reader)
                            : serializer.Deserialize<TEntity>(reader);
                        break;
                }
            }

            return response;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (value is not ODataResponse<TEntity> response)
            {
                writer.WriteEndObject();
                return;
            }

            if (response.Context is not null)
            {
                writer.WritePropertyName(oDataContext);
                writer.WriteValue(response.Context);
            }

            if (response.Count is not null)
            {
                writer.WritePropertyName(oDataCount);
                writer.WriteValue(response.Count);
            }

            if (response.Value is not null)
            {
                writer.WritePropertyName(oDataValue);
                serializer.Serialize(writer, response.Value);
            }

            if (response.NextLink is not null)
            {
                writer.WritePropertyName(oDataNextLink);
                writer.WriteValue(response.NextLink);
            }

            writer.WriteEndObject();
        }
    }
}