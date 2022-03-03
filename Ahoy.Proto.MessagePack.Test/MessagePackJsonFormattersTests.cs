using MessagePack;
using Newtonsoft.Json;
using NUnit.Framework;
using Ahoy.Proto.MessagePack;
using System;
using System.Collections.Generic;
using Ahoy.MessagePack.NewtonsoftJson;
using Ahoy.MessagePack.SystemJson;
using Proto;

namespace Ahoy.Proto.MessagePackTest
{
    public class MessagePackJsonFormattersTests
    {
        public class JsonClass
        {
            public string Name { get; set; }
        }

        public class JsonStruct
        {
            public string Name { get; set; }
        }

        [MessagePackObject]
        public class NestedProtoNewtonsoft
        {
#pragma warning disable MsgPack003 // Use MessagePackObjectAttribute
            [Key(0)]
            [MessagePackFormatter(typeof(NewtonsoftJsonFormatter<JsonClass>))]
            public JsonClass Json1 { get; set; }

            [Key(1)]
            [MessagePackFormatter(typeof(NewtonsoftJsonFormatter<JsonStruct>))]
            public JsonStruct Json2 { get; set; }

            [Key(2)]
            public bool BoolTest { get; set; }
#pragma warning restore MsgPack003 // Use MessagePackObjectAttribute
        }

        [MessagePackObject]
        public class NestedProtoSystemText
        {
#pragma warning disable MsgPack003 // Use MessagePackObjectAttribute
            [Key(0)]
            [MessagePackFormatter(typeof(SystemTextJsonFormatter<JsonClass>))]
            public JsonClass Json1 { get; set; }

            [Key(1)]
            [MessagePackFormatter(typeof(SystemTextJsonFormatter<JsonStruct>))]
            public JsonStruct Json2 { get; set; }
#pragma warning restore MsgPack003 // Use MessagePackObjectAttribute
        }

        [JsonObject]
        public class GameServerConnectData
        {
            public string Domain;
            public string UIDApp, UIDServer;
            public string WebsocketPublicURI;
        }

        [JsonObject]
        public class GameServerConnectState
        {
            public bool TrafficEnabled = true;
        }

        [JsonObject]
        public class GameServerStats
        {
            public long ConnectionCount;

            public long GameCountTotal;
            public long GamesCreated;

            public long MemoryAllocated;
            public long WorkingSet64;
        }

        [MessagePackObject]
#pragma warning disable MsgPack003 // Use MessagePackObjectAttribute
        public class GameServer
        {
            [Key(0)]
            [MessagePackFormatter(typeof(NewtonsoftJsonFormatter<GameServerConnectData>))]
            public GameServerConnectData ConnectData { get; init; }
            [Key(1)]
            public DateTime RegistrationTime { get; init; } = DateTime.UtcNow;
            [Key(2)]
            [MessagePackFormatter(typeof(PIDFormatter))]
            public PID ControlActorPID { get; init; }
            [Key(3)]
            public DateTime LastStatsReportTime { get; set; } = DateTime.UtcNow;
            [Key(4)]
            [MessagePackFormatter(typeof(NewtonsoftJsonFormatter<GameServerStats>))]
            public GameServerStats LastStatsReport { get; set; }

            /// <summary>
            /// If game server is active (got the first stats report by server)
            /// </summary>
            [Key(5)]
            public bool IsTrafficEnabled { get; set; }

            public void AddStatsReport(GameServerStats stats)
            {
                LastStatsReport = stats;
                LastStatsReportTime = DateTime.UtcNow;
            }

            public void SetTrafficEnabled(bool enabled) => IsTrafficEnabled = enabled;
        }
#pragma warning restore MsgPack003 // Use MessagePackObjectAttribute

        [MessagePackObject]
        public record GetAllServersRes
        {
            [Key(0)]
            public List<GameServer> Servers { get; init; }
        }

        [Test]
        public void NewtonsoftJson()
        {
            NestedProtoNewtonsoft pack = new NestedProtoNewtonsoft()
            {
                Json1 = new JsonClass
                {
                    Name = "Ahoy",
                },
                Json2 = new JsonStruct
                {
                    Name = "Games",
                },
                BoolTest = true,
            };
            var bytes = MessagePackSerializer.Serialize(pack);
            var packOut = MessagePackSerializer.Deserialize<NestedProtoNewtonsoft>(bytes);
            Assert.AreEqual(pack.Json1.Name, packOut.Json1.Name);
            Assert.AreEqual(pack.Json2.Name, packOut.Json2.Name);
            Assert.AreEqual(pack.BoolTest, packOut.BoolTest);
        }

        [Test]
        public void ComplexJson()
        {
            var pack = new GetAllServersRes()
            {
                Servers = new List<GameServer>()
                {
                    new GameServer()
                    {
                        ConnectData = new GameServerConnectData()
                        {
                            Domain = "domain",
                            UIDApp = "uidapp",
                            UIDServer = "uidserver",
                            WebsocketPublicURI = "uri",
                        },
                        ControlActorPID = new PID("addr", "id"),
                    },
                },
            };
            pack.Servers[0].AddStatsReport(new GameServerStats()
            {
                ConnectionCount = 10,
                GameCountTotal = 15,
                GamesCreated = 1500,
                MemoryAllocated = 123123,
                WorkingSet64 = 234234,
            });
            pack.Servers[0].SetTrafficEnabled(true);

            var bytes = MessagePackSerializer.Serialize(pack);
            var packOut = MessagePackSerializer.Deserialize<GetAllServersRes>(bytes);
            Assert.AreEqual(pack.Servers[0].IsTrafficEnabled, packOut.Servers[0].IsTrafficEnabled);
            Assert.AreEqual(pack.Servers[0].LastStatsReport.WorkingSet64, packOut.Servers[0].LastStatsReport.WorkingSet64);
        }

        [Test]
        public void SystemTextJson()
        {
            NestedProtoSystemText pack = new NestedProtoSystemText()
            {
                Json1 = new JsonClass
                {
                    Name = "Ahoy",
                },
                Json2 = new JsonStruct
                {
                    Name = "Games",
                },
            };
            var bytes = MessagePackSerializer.Serialize(pack);
            var packOut = MessagePackSerializer.Deserialize<NestedProtoSystemText>(bytes);
            Assert.AreEqual(pack.Json1.Name, packOut.Json1.Name);
            Assert.AreEqual(pack.Json2.Name, packOut.Json2.Name);
        }
    }
}