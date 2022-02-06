using MessagePack;
using MessagePack.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Thrift.Protocol;

namespace Ahoy.ProtoMessagePackThriftSupport
{
    public class ThriftResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new ThriftResolver();
        private ThriftResolver() { }
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (formatterMap.TryGetValue(typeof(T), out var formatter))
                return formatter as IMessagePackFormatter<T>;

            // Not a thrift struct.
            if (typeof(TBase).IsAssignableFrom(typeof(T)) == false)
                return null;

            var newFormatter = Activator.CreateInstance(typeof(ThriftFormatter<>).MakeGenericType(typeof(T)))
                as IMessagePackFormatter<T>;
            formatterMap.TryAdd(typeof(T), newFormatter);
            return newFormatter;
        }

        private static readonly ConcurrentDictionary<Type, object> formatterMap = new ConcurrentDictionary<Type, object>();
    }
}
