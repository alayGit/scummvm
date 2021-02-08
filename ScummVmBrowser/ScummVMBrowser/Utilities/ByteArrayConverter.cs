using ManagedCommon.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class ByteArrayConverter : JsonConverter
{
    private IByteEncoder _byteEncoder;

    public ByteArrayConverter(IByteEncoder byteEncoder)
    {
        _byteEncoder = byteEncoder;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(byte[]);
    }

    public override bool CanRead { get { return false; } } 

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JValue jValue = new JValue(_byteEncoder.AssciiByteEncode((byte[]) value));

        jValue.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}