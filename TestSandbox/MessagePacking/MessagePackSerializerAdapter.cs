using MessagePack;

namespace TestSandbox.MessagePacking
{
    public class MessagePackSerializerAdapter: IMessageSerializer
    {
        /// <inheritdoc/>
        public byte[] Serialize<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }
    }
}
