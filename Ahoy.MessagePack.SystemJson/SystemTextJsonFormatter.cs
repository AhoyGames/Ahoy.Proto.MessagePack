using MessagePack;
using MessagePack.Formatters;
using System.Text.Json;

namespace Ahoy.MessagePack.SystemJson
{
    /// <summary>
    /// Serializes given value using System.Text.Json library.
    /// </summary>
    public class SystemTextJsonFormatter<T> : IMessagePackFormatter<T> where T : new()
    {
        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            var jsonStr = JsonSerializer.Serialize(value);
            writer.Write(jsonStr);
        }

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return default;

            var jsonStr = reader.ReadString();
            return JsonSerializer.Deserialize<T>(jsonStr);
        }
    }
}