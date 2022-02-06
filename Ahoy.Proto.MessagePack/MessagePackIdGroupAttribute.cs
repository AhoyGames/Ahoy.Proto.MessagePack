using MessagePack;
using System;

namespace Ahoy.Proto.MessagePack
{
    /// <summary>
    /// The value given to this attribute is added to all classes with MessagePackIdAttribute attribute underneath it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MessagePackIdGroupAttribute : MessagePackObjectAttribute
    {
        public int IdGroup { get; }

        public MessagePackIdGroupAttribute(int IdGroup)
        {
            this.IdGroup = IdGroup;
        }
    }
}
