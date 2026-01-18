using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using NLog;

namespace SymOntoClay.CoreHelper.SerializerAdapters
{
    public class MessagePackSerializerAdapter : ISerializerAdapter
    {
        /// <inheritdoc/>
        public byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] data)
        {
            return MessagePackSerializer.Deserialize<T>(data);
        }
    }
}

/*
var options = MessagePackSerializerOptions.Standard
    .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance);

byte[] dump = MessagePackSerializer.Serialize(complexObject, options);
 */
