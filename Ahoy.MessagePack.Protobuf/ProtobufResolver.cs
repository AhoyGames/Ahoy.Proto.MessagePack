using MessagePack;
using MessagePack.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;

namespace Ahoy.MessagePack.Protobuf
{
    public class ProtobufResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new ProtobufResolver();
        private ProtobufResolver() { }
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (formatterMap.TryGetValue(typeof(T), out var formatter))
                return formatter as IMessagePackFormatter<T>;

            // Not a protobuf struct.
            if (typeof(IMessage).IsAssignableFrom(typeof(T)) == false)
                return null;

            var newFormatter = Activator.CreateInstance(typeof(ProtobufFormatter<>).MakeGenericType(typeof(T)))
                as IMessagePackFormatter<T>;
            formatterMap.TryAdd(typeof(T), newFormatter);
            return newFormatter;
        }

        private static readonly ConcurrentDictionary<Type, object> formatterMap = new ConcurrentDictionary<Type, object>();
    }
}
