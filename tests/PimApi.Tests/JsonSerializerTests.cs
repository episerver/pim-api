using Newtonsoft.Json;
using static PimApi.Tests.TestSetup;
using NJson = PimApi.JsonSerialization.NewtonsoftJsonSerializer;
using SJson = PimApi.JsonSerialization.SystemTextJsonSerialzer;

namespace PimApi.Tests;

[ExcludeFromCodeCoverage]
[TestFixture(Category = nameof(JsonSerialization))]
[Parallelizable(ParallelScope.All)]
public class JsonSerializerTests
{
    [TestCase(SystemTextJsonSerializer, true)]
    [TestCase(NewtonsoftJsonSerializer, true)]
    [TestCase(SystemTextJsonSerializer, false)]
    [TestCase(NewtonsoftJsonSerializer, false)]
    [Test]
    public void ShouldSerializeConnectionInformationToString(string serializerKey, bool withType)
    {
        var jsonSerializer = GetJsonSerializer(serializerKey);
        var connectionInformation = new ConnectionInformation
        {
            AppKey = nameof(ConnectionInformation.AppKey),
            AppSecret = nameof(ConnectionInformation.AppSecret)
        };

        var type = withType ? typeof(ConnectionInformation) : null;
        var serializedData = jsonSerializer.Serialize(connectionInformation, type);

        serializedData.Should().NotBeNullOrWhiteSpace();

        var deserializedData = jsonSerializer.Deserialize<ConnectionInformation>(serializedData);

        deserializedData.Should().NotBeNull();

        deserializedData!.AppKey.Should().Be(connectionInformation.AppKey);
        deserializedData!.AppSecret.Should().Be(connectionInformation.AppSecret);
    }

    [TestCase(SystemTextJsonSerializer)]
    [TestCase(NewtonsoftJsonSerializer)]
    [Test]
    public async Task ShouldSerializeConnectionInformationToStringAsync(string serializerKey)
    {
        var jsonSerializer = GetJsonSerializer(serializerKey);
        var connectionInformation = new ConnectionInformation
        {
            AppKey = nameof(ConnectionInformation.AppKey),
            AppSecret = nameof(ConnectionInformation.AppSecret)
        };

        var serializedData = jsonSerializer.Serialize(
            connectionInformation,
            typeof(ConnectionInformation)
        );

        serializedData.Should().NotBeNullOrWhiteSpace();

        var deserializedData = await jsonSerializer.DeserializeAsync<ConnectionInformation>(
            GenerateStreamFromString(serializedData!)
        );

        deserializedData.Should().NotBeNull();

        deserializedData!.AppKey.Should().Be(connectionInformation.AppKey);
        deserializedData!.AppSecret.Should().Be(connectionInformation.AppSecret);
    }

    private static IJsonSerializer GetJsonSerializer(string serializerKey) =>
        serializerKey == NewtonsoftJsonSerializer
            ? new NJson(new[] { new TestConverter1() })
            : new SJson(new[] { new TestConverter2() });

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }

    private class TestConverter1 : JsonConverter
    {
        public override bool CanConvert(Type objectType) => false;

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        ) => throw new NotImplementedException();

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
        ) => throw new NotImplementedException();
    }

    private class TestConverter2 : System.Text.Json.Serialization.JsonConverter<TestConverter1>
    {
        public override TestConverter1? Read(
            ref System.Text.Json.Utf8JsonReader reader,
            Type typeToConvert,
            System.Text.Json.JsonSerializerOptions options
        ) => throw new NotImplementedException();

        public override void Write(
            System.Text.Json.Utf8JsonWriter writer,
            TestConverter1 value,
            System.Text.Json.JsonSerializerOptions options
        ) => throw new NotImplementedException();
    }
}
