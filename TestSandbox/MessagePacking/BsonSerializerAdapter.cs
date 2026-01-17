using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

namespace TestSandbox.MessagePacking
{
    public class BsonSerializerAdapter: IMessageSerializer
    {
        /// <inheritdoc/>
        public byte[] Serialize<T>(T obj)
        {
            using var ms = new MemoryStream();
            using var writer = new BsonDataWriter(ms);
            var serializer = new JsonSerializer();
            serializer.Serialize(writer, obj);
            return ms.ToArray();
        }
    }
}
