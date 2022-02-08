using MessagePack;
using MessagePack.Formatters;
using Proto.Cluster;

namespace Ahoy.Proto.MessagePack
{
    // ClusterIdentity Formatter.
    public class ClusterIdentityFormatter : IMessagePackFormatter<ClusterIdentity>
    {
        public void Serialize(ref MessagePackWriter writer, ClusterIdentity value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.Write(value.Kind);
            writer.Write(value.Identity);
        }

        public ClusterIdentity Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return null;

            options.Security.DepthStep(ref reader);

            var kind = reader.ReadString();
            var identity = reader.ReadString();

            reader.Depth--;
            return new ClusterIdentity()
            {
                Kind = kind,
                Identity = identity,
            };
        }
    }
}
