using Google.Protobuf;
using MessagePack;
using MessagePack.Formatters;
using System.Buffers;

namespace Ahoy.MessagePack.Protobuf
{
    public class ProtobufFormatter<T> : IMessagePackFormatter<T> where T : class, IMessage<T>, new()
    {
        static MessageParser<T> parser = new MessageParser<T>(() => new T());

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return null;

            ReadOnlySequence<byte>? bytes = reader.ReadBytes();
            if (bytes.HasValue == false)
                return null;
            return parser.ParseFrom(bytes.Value);
        }

        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.Write(value.ToByteArray());
        }
    }
}