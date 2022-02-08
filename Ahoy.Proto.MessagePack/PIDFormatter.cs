using MessagePack;
using MessagePack.Formatters;
using Proto;

namespace Ahoy.Proto.MessagePack
{
    // PID Formatter.
    public class PIDFormatter : IMessagePackFormatter<PID>
    {
        public void Serialize(ref MessagePackWriter writer, PID value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.Write(value.Address);
            writer.Write(value.Id);
        }

        public PID Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return null;

            options.Security.DepthStep(ref reader);

            var address = reader.ReadString();
            var id = reader.ReadString();

            reader.Depth--;
            return new PID(address, id);
        }
    }
}
