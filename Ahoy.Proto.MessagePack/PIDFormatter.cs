﻿using MessagePack;
using MessagePack.Formatters;

namespace Proto.Serializer.MessagePack
{
    // PID Formatter.
    public class PIDFormatter : IMessagePackFormatter<PID>
    {
        public void Serialize(ref MessagePackWriter writer, PID value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.Write(value.Address);
            writer.Write(value.Id);
        }

        public PID Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
                return null;

            var address = reader.ReadString();
            var id = reader.ReadString();
            return new PID(address, id);
        }
    }
}