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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using IFormatterResolver = global::MessagePack.IFormatterResolver;
using MessagePackSerializer = global::MessagePack.MessagePackSerializer;

namespace Ahoy.Proto.MessagePack
{
    /// <summary>
    /// Used for constructing type info maps required by the ProtoMessagePackSerializer.
    /// </summary>
    public class ProtoMessagePackTypeRegistry
    {
        internal ImmutableDictionary<string, Type> typenameToType = ImmutableDictionary<string, Type>.Empty;
        internal ImmutableDictionary<Type, string> typeToTypename = ImmutableDictionary<Type, string>.Empty;

        /// <summary>
        /// Scans the given assembly for types marked with MessagePackId or MessagePackObject.
        /// </summary>
        public void AddTypesFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Skip the already scanned types, in case the AddTypesFromAssembly is called for the same assembly multiple times.
                if (typeToTypename.ContainsKey(type))
                    continue;

                MessagePackIdAttribute idAttribute = type.GetCustomAttribute<MessagePackIdAttribute>();
                if (idAttribute != null)
                {
                    int id = idAttribute.Id;
                    MessagePackIdGroupAttribute idAttributeGroup = type.DeclaringType?.GetCustomAttribute<MessagePackIdGroupAttribute>();
                    if (idAttributeGroup != null)
                        id += idAttributeGroup.IdGroup;
                    string typename = id.ToString();

                    // Check for duplicate.
                    if (typenameToType.ContainsKey(typename))
                        throw new InvalidOperationException(
                            $"Duplicate MessagePackId {typename} - {typenameToType[typename]} and {type}");

                    AddType(typename, type);
                    continue;
                }
                MessagePackObjectAttribute messagePackObjectAttribute = type.GetCustomAttribute<MessagePackObjectAttribute>();
                if (messagePackObjectAttribute != null)
                {
                    // Use the AssemblyName+TypeName as the source typename.
                    string typenameInputStr = $"{type.Assembly.GetName().Name} {type.FullName}";
                    // Use Base64 encoded version of 64 bit stable hashcode of the typenameInputStr.
                    long typenameHashcode = typenameInputStr.GetStableHashCode64();
                    string typenameBase64 = Convert.ToBase64String(BitConverter.GetBytes(typenameHashcode));
                    // Drop the '=' padding characters at the end.
                    // NOTE: We simply use this as an identifier, and never convert to string back
                    // from the Base64 representation, therefore it is safe to drop the padding chars.
                    string typename = typenameBase64.TrimEnd('=');

                    // Check for duplicate.
                    if (typenameToType.ContainsKey(typename))
                        throw new InvalidOperationException(
                            $"Duplicate MessagePackObject type hashcode. {typename} - {typenameToType[typename]} and {type}. " +
                            $"Either use MessagePackId attribute or simply try renaming one of the types.");

                    AddType(typename, type);
                    continue;
                }
            }
        }

        /// <summary>
        /// Adds a type manually.
        /// Typename is essentially the type identifier utilized for deserialization.
        /// </summary>
        public void AddType(string typename, Type type)
        {
            // Check for duplicate.
            if (typenameToType.ContainsKey(typename))
                throw new InvalidOperationException($"Duplicate MessagePack typename {typename} - {typenameToType[typename]} and {type}");

            typenameToType = typenameToType.Add(typename, type);
            typeToTypename = typeToTypename.Add(type, typename);
        }
    }

    public class ProtoMessagePackSerializer : ISerializer
    {
        private readonly Dictionary<string, Type> _typenameToType;
        private readonly Dictionary<Type, string> _typeToTypename;
        private readonly ConcurrentDictionary<Type, bool> _canSerializeTypeMap = new();
        private readonly MessagePackSerializerOptions _serializerOptions;
        private delegate ByteString DelegateCreateByteString(ReadOnlyMemory<byte> bytes);
        private readonly DelegateCreateByteString _attachBytesDelegate;

        public ProtoMessagePackSerializer(
            ProtoMessagePackTypeRegistry typeRegistry,
            List<IMessagePackFormatter> formatters = null,
            List<IFormatterResolver> resolvers = null)
        {
            // Copy the data from type registry
            _typenameToType = new(typeRegistry.typenameToType);
            _typeToTypename = new(typeRegistry.typeToTypename);

            // Get the private static AttachBytes method to construct ByteString
            // instances without doing copies.
            var _attachBytesMethod = typeof(ByteString).GetMethod(
                "AttachBytes",
                BindingFlags.Static | BindingFlags.NonPublic,
                null, CallingConventions.Standard,
                new Type[] { typeof(ReadOnlyMemory<byte>) },
                null);
            if (_attachBytesMethod == null)
                throw new InvalidOperationException("AttachBytes not found.");
            _attachBytesDelegate = (DelegateCreateByteString)Delegate.CreateDelegate(
                typeof(DelegateCreateByteString),
                null,
                _attachBytesMethod);

            // Construct the final resolver list.
            var finalResolvers = new List<IFormatterResolver>(20);
            if (resolvers != null)
                finalResolvers.AddRange(resolvers);
            // Support immutable collections
            finalResolvers.Add(ImmutableCollectionResolver.Instance);
            finalResolvers.Add(NativeDateTimeResolver.Instance);
            finalResolvers.Add(NativeGuidResolver.Instance);
            // Standard resolver
            finalResolvers.Add(StandardResolver.Instance);

            var builtinFormatters = new List<IMessagePackFormatter>()
            {
                new PIDFormatter(),
                new ClusterIdentityFormatter(),
            };

            var resolver = CompositeResolver.Create(
                (formatters?.Union(builtinFormatters) ?? builtinFormatters).ToList(),
                finalResolvers);

            _serializerOptions = MessagePackSerializerOptions.Standard
                .WithResolver(resolver);
        }

        bool ISerializer.CanSerialize(object obj)
        {
            var type = obj.GetType();
            bool canSerialize;
            if (_canSerializeTypeMap.TryGetValue(type, out canSerialize))
                return canSerialize;

            canSerialize = false;
            // If this serializer knows the given ID, then use it for serialization.
            if (_typeToTypename.ContainsKey(type))
                canSerialize = true;
            // Cache this type.
            _canSerializeTypeMap[type] = canSerialize;
            return canSerialize;
        }

        object ISerializer.Deserialize(ByteString bytes, string typeName)
        {
            if (_typenameToType.TryGetValue(typeName, out var serializedType))
            {
                return MessagePackSerializer.Deserialize(
                    serializedType,
                    bytes.ToByteArray(),
                    options: _serializerOptions);
            }
            throw new InvalidOperationException($"Unknown typename: {typeName}");
        }

        string ISerializer.GetTypeName(object message)
        {
            return _typeToTypename[message.GetType()];
        }

        ByteString ISerializer.Serialize(object obj)
        {
            byte[] bytes = MessagePackSerializer.Serialize(
                obj.GetType(),
                obj,
                options: _serializerOptions);
            return _attachBytesDelegate(bytes);
        }
    }
}
