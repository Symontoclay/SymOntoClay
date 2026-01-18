using MessagePack;
using SymOntoClay.CoreHelper;

namespace TestSandbox.MessagePacking
{
    public class MessagePackSerializerAdapter: ISerializerAdapter
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
