using MessagePack;
using MessagePack.Formatters;
using System.Buffers;
using Thrift.Protocol;
using Thrift.Transport;

namespace Ahoy.ProtoMessagePackThriftSupport
{
    public class ThriftFormatter<T> : IMessagePackFormatter<T> where T : class, TBase, new()
    {
        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            var buffer = new TMemoryBuffer();
            var protocol = new TCompactProtocol(buffer);
            value.Write(protocol);

            writer.Write(buffer.GetBuffer());
        }

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return null;

            ReadOnlySequence<byte>? dataBytes = reader.ReadBytes();

            if (dataBytes.HasValue == false)
                return null;

            var buffer = new TMemoryBuffer(dataBytes.Value.ToArray());
            var protocol = new TCompactProtocol(buffer);

            var data = new T();
            data.Read(protocol);
            return data;
        }
    }
}
