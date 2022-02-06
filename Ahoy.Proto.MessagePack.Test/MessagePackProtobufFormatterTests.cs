using MessagePack;
using NUnit.Framework;
using Messages;
using Ahoy.MessagePackProtobufFormatter;
using MessagePack.Resolvers;

namespace Proto.Serializer.MessagePackTest
{
    public class MessagePackProtobufFormatterTests
    {
        [MessagePackObject]
        public class NestedProto1
        {
            [Key(1)]
            public string? Name { get; set; }

#pragma warning disable MsgPack003 // Use MessagePackObjectAttribute
            [Key(2)]
            [MessagePackFormatter(typeof(ProtobufFormatter<TestProto1>))]
            public TestProto1? Proto1 { get; set; }
#pragma warning restore MsgPack003 // Use MessagePackObjectAttribute
        }

        [MessagePackObject]
        public class NestedProto2
        {
            [Key(1)]
            public string? Name { get; set; }

#pragma warning disable MsgPack003 // Use MessagePackObjectAttribute
            [Key(2)]
            public TestProto1? Proto1 { get; set; }
#pragma warning restore MsgPack003 // Use MessagePackObjectAttribute
        }

        [Test]
        public void TestNestedProto()
        {
            NestedProto1 pack = new NestedProto1()
            {
                Name = "Ahoy",
                Proto1 = new TestProto1
                {
                    Name = "Ahoy",
                    Surname = "Games",
                    Age = 8,
                },
            };
            var bytes = MessagePackSerializer.Serialize(pack);
            var packOut = MessagePackSerializer.Deserialize<NestedProto1>(bytes);
            Assert.AreEqual(pack.Name, packOut.Name);
            Assert.AreEqual(pack.Proto1.Name, packOut.Proto1.Name);
            Assert.AreEqual(pack.Proto1.Surname, packOut.Proto1.Surname);
            Assert.AreEqual(pack.Proto1.Age, packOut.Proto1.Age);
        }

        [Test]
        public void TestNestedProtoResolver()
        {
            NestedProto2 pack = new NestedProto2()
            {
                Name = "Ahoy",
                Proto1 = new TestProto1
                {
                    Name = "Ahoy",
                    Surname = "Games",
                    Age = 8,
                },
            };
            var resolver = CompositeResolver.Create(
                ProtobufResolver.Instance,
                StandardResolver.Instance);
            var options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver);
            var bytes = MessagePackSerializer.Serialize(pack, options: options);
            var packOut = MessagePackSerializer.Deserialize<NestedProto1>(bytes);
            Assert.AreEqual(pack.Name, packOut.Name);
            Assert.AreEqual(pack.Proto1.Name, packOut.Proto1.Name);
            Assert.AreEqual(pack.Proto1.Surname, packOut.Proto1.Surname);
            Assert.AreEqual(pack.Proto1.Age, packOut.Proto1.Age);
        }
    }
}