using MessagePack;
using System;

namespace Ahoy.Proto.MessagePack
{
    /// <summary>
    /// Defines the ID used to identify this type when serialized via Proto.Serializer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class MessagePackIdAttribute : MessagePackObjectAttribute
    {
        public int Id { get; }

        public MessagePackIdAttribute(int Id)
        {
            this.Id = Id;
        }
    }
}
