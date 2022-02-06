using MessagePack.Formatters;
using IFormatterResolver = global::MessagePack.IFormatterResolver;

namespace Ahoy.Proto.MessagePack
{
    public class ProtoActorResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new ProtoActorResolver();
        private ProtoActorResolver() { }
        public IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.Formatter;

        private static class FormatterCache<T>
        {
            public static IMessagePackFormatter<T> Formatter { get; }
            static FormatterCache() => Formatter = (IMessagePackFormatter<T>)ProtoActorResolverGetFormatterHelper.GetFormatter(typeof(T));
        }
    }
}
