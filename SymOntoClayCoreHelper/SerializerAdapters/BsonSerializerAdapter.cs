using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializerAdapters
{
    public class BsonSerializerAdapter : ISerializerAdapter
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

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BsonDataReader(ms);
            var serializer = new JsonSerializer();
            return serializer.Deserialize<T>(reader);
        }
    }
}
