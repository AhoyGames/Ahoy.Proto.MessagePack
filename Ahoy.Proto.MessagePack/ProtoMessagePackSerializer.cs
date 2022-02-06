using Google.Protobuf;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using Proto.Remote;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using IFormatterResolver = global::MessagePack.IFormatterResolver;
using MessagePackSerializer = global::MessagePack.MessagePackSerializer;

namespace Ahoy.Proto.MessagePack
{
    public class ProtoMessagePackSerializer : ISerializer
    {
        Dictionary<Type, string> typeToName = new Dictionary<Type, string>();
        Dictionary<string, Type> nameToType = new Dictionary<string, Type>();
        HashSet<int> knownIds = new HashSet<int>();
        ConcurrentDictionary<Type, bool> canSerializeTypeMap = new ConcurrentDictionary<Type, bool>();
        private MessagePackSerializerOptions serializerOptions;
        private MethodInfo attachBytesMethod;
        public delegate ByteString DelegateCreateByteString(ReadOnlyMemory<byte> bytes);
        private DelegateCreateByteString attachBytesDelegate;

        public static Dictionary<int, Type> ScanAssemblyForTypes(
            Assembly assembly)
        {
            Dictionary<int, Type> types = new Dictionary<int, Type>();
            foreach (Type type in assembly.GetTypes())
            {
                int? messagePackId = GetMessagePackId(type);
                if (messagePackId.HasValue)
                {
                    // Check for duplicate.
                    if (types.ContainsKey(messagePackId.Value))
                        throw new Exception($"Duplicate MessagePackId {messagePackId.Value} - {types[messagePackId.Value]} and {type}");

                    types.Add(messagePackId.Value, type);
                }
            }
            return types;
        }

        public ProtoMessagePackSerializer(
            List<IMessagePackFormatter> formatters,
            List<IFormatterResolver> resolvers,
            params Dictionary<int, Type>[] idToTypes)
        {
            // Get the private static AttachBytes method to construct ByteString
            // instances without doing copies.
            attachBytesMethod = typeof(ByteString).GetMethod(
                "AttachBytes",
                BindingFlags.Static | BindingFlags.NonPublic,
                null, CallingConventions.Standard,
                new Type[] { typeof(ReadOnlyMemory<byte>) },
                null);
            if (attachBytesMethod == null)
                throw new InvalidOperationException("AttachBytes not found.");
            attachBytesDelegate = (DelegateCreateByteString)Delegate.CreateDelegate(
                typeof(DelegateCreateByteString),
                null,
                attachBytesMethod);

            // Construct the final resolver list.
            var finalResolvers = new List<IFormatterResolver>(20);
            if (resolvers != null)
                finalResolvers.AddRange(resolvers);
            // Support proto actor PID.
            finalResolvers.Add(ProtoActorResolver.Instance);
            // Support immutable collections
            finalResolvers.Add(ImmutableCollectionResolver.Instance);
            finalResolvers.Add(NativeDateTimeResolver.Instance);
            finalResolvers.Add(NativeGuidResolver.Instance);
            // Standard resolver
            finalResolvers.Add(StandardResolver.Instance);

            var resolver = CompositeResolver.Create(
                formatters ?? new List<IMessagePackFormatter>(),
                finalResolvers);

            serializerOptions = MessagePackSerializerOptions.Standard
                .WithResolver(resolver);

            foreach (var idToType in idToTypes)
            {
                foreach (var item in idToType)
                {
                    var idStr = $"{item.Key}";

                    if (nameToType.ContainsKey(idStr))
                        throw new Exception($"Duplicate MessagePackId {idStr} - {nameToType[idStr]} and {item.Value}");

                    knownIds.Add(item.Key);
                    nameToType.Add(idStr, item.Value);
                    typeToName.Add(item.Value, idStr);
                }
            }
        }

        public bool CanSerialize(object obj)
        {
            var type = obj.GetType();
            bool canSerialize;
            if (canSerializeTypeMap.TryGetValue(type, out canSerialize))
                return canSerialize;

            canSerialize = false;
            // If this serializer knows the given ID, then use it for serialization.
            int? messagePackId = GetMessagePackId(obj.GetType());
            if (messagePackId.HasValue &&
                knownIds.Contains(messagePackId.Value))
                canSerialize = true;
            // Cache this type.
            canSerializeTypeMap[type] = canSerialize;
            return canSerialize;
        }

        private static int? GetMessagePackId(Type type)
        {
            MessagePackIdAttribute idAttribute = type.GetCustomAttribute<MessagePackIdAttribute>();
            if (idAttribute != null)
            {
                int id = idAttribute.Id;
                MessagePackIdGroupAttribute idAttributeGroup = type.DeclaringType?.GetCustomAttribute<MessagePackIdGroupAttribute>();
                if (idAttributeGroup != null)
                    id += idAttributeGroup.IdGroup;
                return id;
            }
            return null;
        }

        public object Deserialize(ByteString bytes, string typeName)
        {
            return Deserialize(bytes.ToByteArray(), typeName);
        }

        public object Deserialize(byte[] bytes, string typeName)
        {
            if (nameToType.TryGetValue(typeName, out var serializedType))
            {
                return MessagePackSerializer.Deserialize(
                    serializedType,
                    bytes,
                    options: serializerOptions);
            }
            throw new Exception($"Unknown typename: {typeName}");
        }

        public string GetTypeName(object message)
        {
            return typeToName[message.GetType()];
        }

        public ByteString Serialize(object obj)
        {
            byte[] bytes = MessagePackSerializer.Serialize(
                obj.GetType(),
                obj,
                options: serializerOptions);
            return attachBytesDelegate(bytes);
        }
    }
}
