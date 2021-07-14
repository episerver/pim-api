using PimApi.Entities;
using PimApi.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using static PimApi.Defaults;

namespace PimApi.JsonSerialization
{
    public class SystemTextJsonODataCollectionConverter<TEntity> : JsonConverter<ODataResponse>
        where TEntity : BaseEntityDto
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.CanBeODataResponse<TEntity>();

        public override ODataResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) { return null; }

            var isCollection =
                typeof(ODataResponse<ICollection<TEntity>>).IsAssignableFrom(typeToConvert)
                || typeof(ODataResponseCollection<TEntity>).IsAssignableFrom(typeToConvert);
            ODataResponse response = isCollection
                ? new ODataResponseCollection<TEntity>()
                : new ODataResponse<TEntity>();

            var propertyName = string.Empty;
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName) { break; }

                propertyName = reader.GetString();
                switch (propertyName)
                {
                    case oDataContext:
                        reader.Read();
                        response.Context = reader.GetString();
                        break;

                    case oDataCount:
                        reader.Read();
                        reader.TryGetInt32(out var count);
                        response.Count = count;
                        break;

                    case oDataNextLink:
                        reader.Read();
                        response.NextLink = reader.GetString();
                        break;

                    case oDataValue:
                        response.Value = isCollection
                            ? JsonSerializer.Deserialize<ICollection<TEntity>>(ref reader, options)!
                            : JsonSerializer.Deserialize<TEntity>(ref reader, options)!;
                        break;
                }
            }

            return response;
        }

        public override void Write(Utf8JsonWriter writer, ODataResponse value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is null)
            {
                writer.WriteEndObject();
                return;
            }

            if (value.Context is not null)
            {
                writer.WriteString(oDataContext, value.Context);
            }

            if (value.Count is not null)
            {
                writer.WriteNumber(oDataCount, value.Count.Value);
            }

            if (value.Value is not null)
            {
                writer.WriteString(oDataValue, JsonSerializer.Serialize(value.Value, value.Value.GetType(), options));
            }

            if (value.NextLink is not null)
            {
                writer.WriteString(oDataNextLink, value.Context);
            }

            writer.WriteEndObject();

            return;
        }
    }
}