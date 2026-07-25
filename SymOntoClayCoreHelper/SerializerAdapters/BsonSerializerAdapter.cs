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
            var serializer = new JsonSerializer();
            return Serialize<T>(obj, serializer);
        }

        public byte[] Serialize<T>(T obj, JsonSerializer serializer)
        {
            using var ms = new MemoryStream();
            using var writer = new BsonDataWriter(ms);
            serializer.Serialize(writer, obj);
            return ms.ToArray();
        }

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] data)
        {
            var serializer = new JsonSerializer();
            return Deserialize<T>(data, serializer);
        }

        public T Deserialize<T>(byte[] data, JsonSerializer serializer)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BsonDataReader(ms);
            return serializer.Deserialize<T>(reader);
        }
    }
}
