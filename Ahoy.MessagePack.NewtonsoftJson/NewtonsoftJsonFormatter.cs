using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json;

namespace Ahoy.MessagePack.NewtonsoftJson
{
    /// <summary>
    /// Serializes given value using Newtonsoft.Json library.
    /// </summary>
    public class NewtonsoftJsonFormatter<T> : IMessagePackFormatter<T> where T : new()
    {
        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            var jsonStr = JsonConvert.SerializeObject(value);
            writer.Write(jsonStr);
        }

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return default;

            var jsonStr = reader.ReadString();
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}