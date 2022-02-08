using MessagePack;
using NUnit.Framework;
using Messages;
using MessagePack.Resolvers;
using Ahoy.MessagePack.Protobuf;
using Proto;
using Ahoy.Proto.MessagePack;
using Proto.Cluster;

namespace Ahoy.Proto.MessagePackTest
{
    public class MessagePackPIDFormatterTests
    {
        [MessagePackObject]
        public class PIDMessagePack
        {
            [Key(0)] public PID pid { get; set; }
            [Key(1)] public PID pidNull { get; set; }
            [Key(2)] public ClusterIdentity identity { get; set; }
            [Key(3)] public ClusterIdentity identityNull { get; set; }
        }

        [MessagePackObject]
        public class PIDMessagePackNested
        {
            [Key(0)] public PIDMessagePack nested { get; set; }
        }

        [Test]
        public void ProtoActorPIDSerialization()
        {
            var serializer = new ProtoMessagePackSerializer(null, null, new Dictionary<int, Type>()
            {
                { 1, typeof(PIDMessagePack) },
            });

            var pack = new PIDMessagePack()
            {
                pid = new PID("address", "id"),
                pidNull = null,
                identity = ClusterIdentity.Create("identity", "kind"),
                identityNull = null,
            };
            var outputBytes = serializer.Serialize(pack);
            var typename = serializer.GetTypeName(pack);
            var packOut = serializer.Deserialize(outputBytes, typename) as PIDMessagePack;
            Assert.AreEqual(packOut.pid.Id, pack.pid.Id);
            Assert.AreEqual(packOut.pid.Address, pack.pid.Address);
            Assert.IsNull(packOut.pidNull);
            Assert.AreEqual(packOut.identity.Kind, pack.identity.Kind);
            Assert.AreEqual(packOut.identity.Identity, pack.identity.Identity);
            Assert.IsNull(packOut.identityNull);
        }

        [Test]
        public void NestedProtoActorPIDSerialization()
        {
            var serializer = new ProtoMessagePackSerializer(null, null, new Dictionary<int, Type>()
            {
                { 1, typeof(PIDMessagePackNested) },
            });

            var pack = new PIDMessagePackNested()
            {
                nested = new PIDMessagePack()
                {
                    pid = new PID("address", "id"),
                    pidNull = null,
                    identity = ClusterIdentity.Create("identity", "kind"),
                    identityNull = null,
                },
            };
            var outputBytes = serializer.Serialize(pack);
            var typename = serializer.GetTypeName(pack);
            var packOut = serializer.Deserialize(outputBytes, typename) as PIDMessagePackNested;
            Assert.AreEqual(packOut.nested.pid.Id, pack.nested.pid.Id);
            Assert.AreEqual(packOut.nested.pid.Address, pack.nested.pid.Address);
            Assert.IsNull(packOut.nested.pidNull);
            Assert.AreEqual(packOut.nested.identity.Kind, pack.nested.identity.Kind);
            Assert.AreEqual(packOut.nested.identity.Identity, pack.nested.identity.Identity);
            Assert.IsNull(packOut.nested.identityNull);
        }
    }
}