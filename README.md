# Ahoy.Proto.MessagePack
MessagePack serializer for Proto.Actor.

Uses https://github.com/neuecc/MessagePack-CSharp library for serialization.

MessagePack + Proto.Actor = 🚀🚀🚀

## Installation
Install `Ahoy.Proto.MessagePack` nuget package.

## How to use
```csharp
ActorSystem actorSystem = new();
actorSystem.Serialization().RegisterSerializer(
  // Must be any value other than 0 or 1. Those are used internally by Proto.Actor.
  serializerId: 2,
  priority: 10,
  new ProtoMessagePackSerializer(
      formatters: null,
      resolvers: null,
      ProtoMessagePackSerializer.ScanAssemblyForTypes(Assembly.GetExecutingAssembly())));
```

## Built-in supported types
It supports all the types neucc's MessagePack-CSharp (https://github.com/neuecc/MessagePack-CSharp#built-in-supported-types) supports.
On top of that it adds the following resolvers automatically:
- ImmutableCollectionResolver
- NativeDateTimeResolver
- NativeGuidResolver

It also automatically adds an internal resolver: ProtoActorResolver. This adds support for the `PID` and `ClusterIdentity` types.

## How to use MessagePack structs from multiple assemblies?
Create ProtoMessagePackSerializer as such:
```csharp
new ProtoMessagePackSerializer(
  formatters: null,
  resolvers: null,
  ProtoMessagePackSerializer.ScanAssemblyForTypes(Assembly.GetExecutingAssembly()),
  ProtoMessagePackSerializer.ScanAssemblyForTypes(Assembly.GetAssembly(typeof(MyType)));
```

## How to define message pack objects?
Must add `[MessagePackId(0)]` attribute instead of `[MessagePackObject]` attribute.
```csharp
[MessagePackId(0)]
public record CreateUser
{
  [Key(0)] public Guid UserId { get; init; }
  [Key(1)] public string Name { get; init; }
  [Key(2)] public int Age { get; init; }
}

[MessagePackId(1)]
public record DeleteUser
{
  [Key(0)] public Guid UserId { get; init; }
}
```
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

## How to use it together with Ahoy.MessagePack.Protobuf
Install `Ahoy.MessagePack.Protobuf` nuget package.

```csharp
actorSystem.Serialization().RegisterSerializer(
  // Must be any value other than 0 or 1. Those are used internally by Proto.Actor.
  serializerId: 2,
  priority: 10,
  new ProtoMessagePackSerializer(
      formatters: null,
      resolvers: new List<MessagePack.IFormatterResolver>()
      {
          // Support Protobuf structs within MessagePack structs.
          ProtobufResolver.Instance,
      },
      ProtoMessagePackSerializer.ScanAssemblyForTypes(Assembly.GetExecutingAssembly()),
      Ahoy.Sockets.MsgPackSerializerCreator.GetTypes(),
      ProtoMessagePackSerializer.ScanAssemblyForTypes(Assembly.GetAssembly(typeof(ReceiveActor)))
  ));
```

## How to use Ahoy.MessagePack.NewtonsoftJson or Ahoy.MessagePack.SystemJson
Install `Ahoy.MessagePack.NewtonsoftJson` and/or `Ahoy.MessagePack.SystemJson` nuget packages.

```csharp
[MessagePackId(1)]
public record TestMessage
{
  [MessagePackFormatter(typeof(NewtonsoftJsonFormatter<MyNewtonsoftJsonType>))]
  [Key(0)] public MyNewtonsoftJsonType Value1 { get; init; }
  
  [MessagePackFormatter(typeof(SystemTextJsonFormatter<MySystemTextJsonType>))]
  [Key(1)] public MySystemTextJsonType Value2 { get; init; }
}
```
