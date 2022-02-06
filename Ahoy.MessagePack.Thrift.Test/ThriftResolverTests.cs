using Ahoy.MessagePack.Thrift;
using HttpServiceData.Leaderboards;
using MessagePack;
using MessagePack.Resolvers;
using NUnit.Framework;
using System.Collections.Generic;

namespace ProtoMessagePackThriftSupportTest
{
    public class ProtoMessagePackThrift
    {
        [MessagePackObject]
        public class MsgPackObjectTest
        {
            [Key(0)]
            public int Key0Test;

#pragma warning disable MsgPack003 // Use MessagePackObjectAttribute
            [Key(1)]
            public LeaderboardConsumedCoinsReq ThriftTest1;

            [Key(2)]
            public LeaderboardUserRep UserRep1;
#pragma warning restore MsgPack003 // Use MessagePackObjectAttribute
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SerializeDeserializeThriftField()
        {
            // Construct the final resolver list.
            var resolver = CompositeResolver.Create(
                ThriftResolver.Instance,
                StandardResolver.Instance);
            var serializerOptions = MessagePackSerializerOptions.Standard
                .WithResolver(resolver);

            var inObj = new MsgPackObjectTest()
            {
                Key0Test = 123,
                ThriftTest1 = new LeaderboardConsumedCoinsReq()
                {
                    AuthToken = "AUTHTOKEN",
                    TotalCoins = 123123,
                    User = "USERID",
                },
                UserRep1 = new LeaderboardUserRep()
                {
                    LeaderboardScore = 321321,
                },
            };

            var serializedBytes = MessagePackSerializer.Serialize(
                inObj.GetType(),
                inObj,
                options: serializerOptions);

            var outObj = MessagePackSerializer.Deserialize<MsgPackObjectTest>(
                serializedBytes,
                options: serializerOptions);

            Assert.AreEqual(inObj.Key0Test, outObj.Key0Test);
            Assert.AreEqual(inObj.ThriftTest1.User, outObj.ThriftTest1.User);
            Assert.AreEqual(inObj.ThriftTest1.AuthToken, outObj.ThriftTest1.AuthToken);
            Assert.AreEqual(inObj.ThriftTest1.TotalCoins, outObj.ThriftTest1.TotalCoins);
            Assert.AreEqual(inObj.ThriftTest1.ConsumedCoins, outObj.ThriftTest1.ConsumedCoins);
            Assert.AreEqual(inObj.UserRep1.LeaderboardScore, outObj.UserRep1.LeaderboardScore);
        }
    }
}