using MessagePack;
using Ahoy.Proto.MessagePack;

namespace Ahoy.Proto.MessagePackTest
{
    [MessagePackId(1)]
    public class TestMsgPack1
    {
        [Key(1)]
        public string Name { get; init; }
        [Key(2)]
        public string Surname { get; init; }
        [Key(3)]
        public int Age { get; init; }
    }

    [MessagePackId(2)]
    public class TestMsgPack2
    {
        [Key(1)]
        public string Name { get; init; }
        [Key(2)]
        public string Surname { get; init; }
        [Key(3)]
        public int Age { get; init; }
    }

    [MessagePackObject]
    public class TestMsgPack3
    {
        [Key(1)]
        public string Name { get; init; }
        [Key(2)]
        public string Surname { get; init; }
        [Key(3)]
        public int Age { get; init; }
    }

    [MessagePackIdGroup(100)]
    public static class MessagePackIdGroupTest1
    {
        [MessagePackId(1)]
        public class TestMsgPack1
        {
            [Key(1)]
            public string Name { get; init; }
            [Key(2)]
            public string Surname { get; init; }
            [Key(3)]
            public int Age { get; init; }
        }
    }

    [MessagePackIdGroup(200)]
    public static class MessagePackIdGroupTest2
    {
        [MessagePackId(1)]
        public class TestMsgPack1
        {
            [Key(1)]
            public string Name { get; init; }
            [Key(2)]
            public string Surname { get; init; }
            [Key(3)]
            public int Age { get; init; }
        }
    }
}