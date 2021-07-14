using Newtonsoft.Json;
using PimApi.Entities;
using PimApi.Extensions;
using System;
using System.Collections.Generic;

namespace PimApi.JsonSerialization
{
    public class NewtonsoftJsonJsonEntityConverter<TEntity> : JsonConverter
       where TEntity : BaseEntityDto, new()
    {
        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type typeToConvert) => typeof(TEntity).IsAssignableFrom(typeToConvert);

        public override object? ReadJson(JsonReader reader, Type typeToConvert, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) { return null; }

            var data = new TEntity();
            var typeProperties = typeToConvert.GetTypeProperties();
            string? propertyName = null;
            object? propertyValue = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject) { break; }

                propertyName = reader.Value?.ToString();

                if (propertyName is null) { throw new Exception("Unable to read propertyName"); }

                typeProperties.TryGetValue(propertyName, out var propertyInfo);

                if (propertyInfo is not null
                    && !propertyInfo.PropertyType.IsPrimitive)
                {
                    reader.Read();
                    propertyInfo.SetValue(data, serializer.Deserialize(reader, propertyInfo.PropertyType));

                    continue;
                }

                reader.Read();
                var stringValue = reader.Value?.ToString();
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        continue;

                    case JsonToken.Boolean:
                        propertyValue = stringValue is string s
                            ? Convert.ToBoolean(s)
                            : null;
                        break;

                    case JsonToken.Null:
                        propertyValue = null;
                        break;

                    case JsonToken.StartArray:
                        propertyValue = serializer.Deserialize(reader, typeof(ICollection<string>));
                        break;

                    // for some reason properties@odata.count and multi string value types shows as propertyName
                    case JsonToken.PropertyName:
                    case JsonToken.String:

                        if (propertyInfo.CanAssignValue(typeof(string)))
                        {
                            propertyValue = stringValue;
                        }
                        else if (propertyInfo.CanAssignValue(typeof(Guid)))
                        {
                            if (Guid.TryParse(stringValue, out var id))
                            {
                                propertyValue = id;
                            }
                        }
                        else
                        {
                            propertyValue = stringValue;
                        }
                        break;

                    case JsonToken.Date:
                        propertyValue = stringValue is not string
                            ? null
                            : propertyInfo.CanAssignValue(typeof(DateTimeOffset))
                                ? DateTimeOffset.Parse(stringValue)
                                : (object?)Convert.ToDateTime(stringValue);
                        break;

                    case JsonToken.Integer: 
                        propertyValue = stringValue is not string
                            ? null
                            : Convert.ToInt32(stringValue);
                        break;

                    case JsonToken.Float:
                        propertyValue =  stringValue is not string 
                            ? null 
                            : Convert.ToDecimal(reader.Value);
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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}