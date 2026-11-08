namespace Dif.Common.UseCaseHandling.Messaging;

using BinarySerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

public static class MessageSerializer
{
    public static byte[] Serialize<T>(T message)
        => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, JsonSerializerOptions.CamelCase));

    public static T? Deserialize<T>(byte[] bytes)
        => JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), JsonSerializerOptions.CamelCase);

    public static byte[] SerializeUsingBinarySerializer<T>(T message, bool isBigEndian)
    {
        var serializer = new BinarySerializer
        {
            Endianness = isBigEndian ? Endianness.Big : Endianness.Little
        };

        using var stream = new MemoryStream();
        serializer.Serialize(stream, message, null);
        return stream.ToArray();
    }

    public static T? DeserializeUsingBinarySerializer<T>(byte[] bytes, bool isBigEndian)
    {
        var serializer = new BinarySerializer
        {
            Endianness = isBigEndian ? Endianness.Big : Endianness.Little
        };

        return serializer.Deserialize<T>(bytes, null);
    }
}

public static class JsonSerializerOptions
{
    public static readonly JsonSerializerSettings CamelCase = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
}