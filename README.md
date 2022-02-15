# Ahoy.Proto.MessagePack
MessagePack serializer for Proto.Actor.

Uses https://github.com/neuecc/MessagePack-CSharp library for serialization.

MessagePack + Proto.Actor = 🚀🚀🚀

## Installation
Install `Ahoy.Proto.MessagePack` nuget package.

## How to use
```csharp
ActorSystem actorSystem = new();

ProtoMessagePackTypeRegistry typeRegistry = new();
typeRegistry.AddTypesFromAssembly(Assembly.GetExecutingAssembly());

actorSystem.Serialization().RegisterSerializer(
    // Must be any value other than 0 or 1. Those are used internally by Proto.Actor.
    serializerId: 2,
    priority: 10,
    new ProtoMessagePackSerializer(typeRegistry));
```

## Built-in supported types
It supports all the types neucc's MessagePack-CSharp (https://github.com/neuecc/MessagePack-CSharp#built-in-supported-types) supports.
On top of that it adds the following resolvers automatically:
- ImmutableCollectionResolver
- NativeDateTimeResolver
- NativeGuidResolver

It also automatically adds an internal resolver: `ProtoActorResolver`. This adds support for the `PID` and `ClusterIdentity` types.

## How to use MessagePack structs from multiple assemblies?
Create ProtoMessagePackSerializer as such:
```csharp
ProtoMessagePackTypeRegistry typeRegistry = new();
typeRegistry.AddTypesFromAssembly(Assembly.GetExecutingAssembly());
typeRegistry.AddTypesFromAssembly(Assembly.GetAssembly(typeof(MyType)));
var myserializer = new ProtoMessagePackSerializer(typeRegistry);
```

## How to define message pack objects?
Like you would do any normal message pack object.
```csharp
[MessagePackObject]
public record CreateUser
{
    [Key(0)] public Guid UserId { get; init; }
    [Key(1)] public string Name { get; init; }
    [Key(2)] public int Age { get; init; }
}

[MessagePackObject]
public record DeleteUser
{
    [Key(0)] public Guid UserId { get; init; }
}
```

In this case, as the typename (proto's type identifier), serializer uses base64 version of a 64 bit hashcode of the type's fullname. These identifiers are used when deserializing the type.
If you would rename the class/record or the assembly it is in, your new binary won't recognize messages from the older version, and vice-versa. To prevent this, utilize MessagePackId() identifier. We also don't provide guarantees that a new version of Ahoy.Proto.MessagePack wouldn't change the auto typename generation method and render it incompatible with older versions.

Auto generated Base64 typenames each take up to 11 bytes of space on wire. If you prefer to minimize on-wire size of the typename, you can use [MessagePackId(?)] attribute with a preferably low value in order to 11 bytes overhead to as-many-bytes-as-digits overhead.

## How to use MessagePackId for static type identifiers
Numeric id passed to MessagePackId attribute must be unique. This id is utilized to determine which type to create during deserialization. As long as the ids are not changed, you can feel free to change the name of the classes and still have new and old versions of your server be compatible with each other.

In case of an id collision, ProtoMessagePackSerializer's constructor will throw an exception with the conflicting ID and typenames.

In order to ease the creation of unique ids, you can place your message types into static classes and place MessagePackIdGroup attribute on that. Example:
```csharp
[MessagePackIdGroup(100)]
public static class UserActorMessages
{
    // Actual id will be: 101
    [MessagePackId(0)]
    public record CreateUser
    {
        [Key(0)] public Guid UserId { get; init; }
        [Key(1)] public string Name { get; init; }
        [Key(2)] public int Age { get; init; }
    }
    
    // Actual id will be: 101
    [MessagePackId(1)]
    public record DeleteUser
    {
        [Key(0)] public Guid UserId { get; init; }
    }
}

[MessagePackIdGroup(200)]
public static class RoomActorMessages
{
    // Actual id will be: 201
    [MessagePackId(0)]
    public record CreateRoom
    {
        [Key(0)] public Guid RoomId { get; init; }
    }
    
    // Actual id will be: 201
    [MessagePackId(1)]
    public record DeleteRoom
    {
        [Key(0)] public Guid RoomId { get; init; }
    }
}
```

You can always mix the usage of messages marked with `[MessagePackId(?)]` and without. If you are doing rolling or blue-green deployment on your Proto.Cluster instances, you are 100% recommended to use `[MessagePackId(?)]` attribute on each message. This will make sure that regardless of how you refactor your code, versions will always be compatible with each other in terms of message serialization. However, outside of those scenerios, simply using `[MessagePackObject]` is easier.

## How to use it together with Ahoy.MessagePack.Protobuf
Install `Ahoy.MessagePack.Protobuf` nuget package.

```csharp
ProtoMessagePackTypeRegistry typeRegistry = new();
typeRegistry.AddTypesFromAssembly(Assembly.GetExecutingAssembly());

actorSystem.Serialization().RegisterSerializer(
    // Must be any value other than 0 or 1. Those are used internally by Proto.Actor.
    serializerId: 2,
    priority: 10,
    new ProtoMessagePackSerializer(
        typeRegistry,
        resolvers: new List<MessagePack.IFormatterResolver>()
        {
            // Support Protobuf structs within MessagePack structs.
            ProtobufResolver.Instance,
        }));
```

## How to use Ahoy.MessagePack.NewtonsoftJson or Ahoy.MessagePack.SystemJson
Install `Ahoy.MessagePack.NewtonsoftJson` and/or `Ahoy.MessagePack.SystemJson` nuget packages.

```csharp
[MessagePackObject]
public record TestMessage
{
    [MessagePackFormatter(typeof(NewtonsoftJsonFormatter<MyNewtonsoftJsonType>))]
    [Key(0)] public MyNewtonsoftJsonType Value1 { get; init; }
    
    [MessagePackFormatter(typeof(SystemTextJsonFormatter<MySystemTextJsonType>))]
    [Key(1)] public MySystemTextJsonType Value2 { get; init; }
}
```
