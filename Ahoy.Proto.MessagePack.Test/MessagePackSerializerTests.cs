using NUnit.Framework;
using Proto.Serializer.MessagePack;
using System.Reflection;

namespace Proto.Serializer.MessagePackTest
{
    public class MessagePackSerializerTests
    {
        private ProtoMessagePackSerializer serializerMessagePack;

        [SetUp]
        public void Setup()
        {
            serializerMessagePack = new ProtoMessagePackSerializer(
                formatters: null,
                resolvers: null,
                ProtoMessagePackSerializer.ScanAssemblyForTypes(Assembly.GetExecutingAssembly()));
        }

        [Test]
        public void CanSerialize()
        {
            Assert.IsTrue(serializerMessagePack.CanSerialize(new TestMsgPack1()));
            Assert.IsTrue(serializerMessagePack.CanSerialize(new TestMsgPack2()));
            Assert.IsFalse(serializerMessagePack.CanSerialize(new TestMsgPack3()));
            Assert.IsFalse(serializerMessagePack.CanSerialize(6969));
        }

        [Test]
        public void GetTypeName()
        {
            Assert.AreEqual("1", serializerMessagePack.GetTypeName(new TestMsgPack1()));
            Assert.AreEqual("2", serializerMessagePack.GetTypeName(new TestMsgPack2()));
        }

        [Test]
        public void Serialize()
        {
            var data = new TestMsgPack1()
            {
                Name = "Ahoy",
                Surname = "Games",
                Age = 8,
            };
            var bytes = serializerMessagePack.Serialize(data);
            var typename = serializerMessagePack.GetTypeName(data);
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
            var dataOut = serializerMessagePack.Deserialize(bytes, typename);
            Assert.IsTrue(dataOut is TestMsgPack1);
            Assert.AreEqual(data.Name, ((TestMsgPack1)dataOut).Name);
            Assert.AreEqual(data.Surname, ((TestMsgPack1)dataOut).Surname);
            Assert.AreEqual(data.Age, ((TestMsgPack1)dataOut).Age);
        }

        [Test]
        public void Serialize2()
        {
            var data = new TestMsgPack2()
            {
                Name = "Ahoy",
                Surname = "Games",
                Age = 8,
            };
            var bytes = serializerMessagePack.Serialize(data);
            var typename = serializerMessagePack.GetTypeName(data);
            var dataOut = serializerMessagePack.Deserialize(bytes, typename);
            Assert.IsTrue(dataOut is TestMsgPack2);
        }
    }
}