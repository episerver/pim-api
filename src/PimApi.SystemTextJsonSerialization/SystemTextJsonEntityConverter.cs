using PimApi.Entities;
using PimApi.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PimApi.JsonSerialization
{
    public class SystemTextJsonEntityConverter<TEntity> :
        JsonConverter<TEntity>
        where TEntity : BaseEntityDto, new()
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(TEntity).IsAssignableFrom(typeToConvert);

        public override TEntity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) { return null; }

            var data = new TEntity();
            var typeProperties = typeToConvert.GetTypeProperties();
            string? propertyName = null;
            object? propertyValue = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) { break; }

                propertyName = reader.GetString();

                if (propertyName is null) { throw new Exception("Unable to read propertyName"); }

                typeProperties.TryGetValue(propertyName, out var propertyInfo);

                if (propertyInfo is not null
                    && !propertyInfo.PropertyType.IsPrimitive)
                {
                    propertyInfo.SetValue(data, JsonSerializer.Deserialize(ref reader, propertyInfo.PropertyType, options));

                    continue;
                }

                reader.Read();

                switch (reader.TokenType)
                {
                    case JsonTokenType.Comment: continue;

                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        propertyValue = reader.GetBoolean();
                        break;

                    case JsonTokenType.Null:
                        propertyValue = null;
                        break;

                    case JsonTokenType.StartArray:
                        propertyValue = JsonSerializer.Deserialize(ref reader, typeof(ICollection<string>), options);
                        break;

                    case JsonTokenType.String:
                        var stringValue = reader.GetString();
                        if (propertyInfo.CanAssignValue(typeof(Guid)))
                        {
                            if (Guid.TryParse(stringValue, out var id))
                            {
                                propertyValue = id;
                            }
                        }
                        else if (propertyInfo.CanAssignValue(typeof(DateTimeOffset)))
                        {
                            if (DateTimeOffset.TryParse(stringValue, out var date))
                            {
                                propertyValue = date;
                            }
                        }
                        else if (propertyInfo.CanAssignValue(typeof(DateTime)))
                        {
                            if (DateTime.TryParse(stringValue, out var date))
                            {
                                propertyValue = date;
                            }
                        }
                        else
                        {
                            propertyValue = stringValue;
                        }
                        break;

                    case JsonTokenType.Number:
                        if (propertyInfo.CanAssignValue(typeof(int)))
                        {
                            propertyValue = reader.GetInt32();
                        }
                        else if (propertyInfo.CanAssignValue(typeof(long)))
                        {
                            propertyValue = reader.GetInt64();
                        }
                        else if (propertyInfo.CanAssignValue(typeof(decimal)))
                        {
                            propertyValue = reader.GetDecimal();
                        }
                        else if (propertyInfo.CanAssignValue(typeof(double)))
                        {
                            propertyValue = reader.GetDouble();
                        }
                        else
                        {
                            propertyValue = reader.GetDecimal();
                        }
                        break;

                    default:
                        throw new Exception("Unable to convert token type" + reader.TokenType);
                }

                // map unknown values to PropertyBag
                if (propertyInfo is null)
                {
                    data.PropertyBag[propertyName] = propertyValue;

                    continue;
                }

                propertyInfo.SetValue(data, propertyValue);
            }

            return data;
        }

        public override void Write(Utf8JsonWriter writer, TEntity value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}