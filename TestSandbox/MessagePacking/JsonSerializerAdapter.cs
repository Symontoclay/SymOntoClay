using Newtonsoft.Json;
using SymOntoClay.CoreHelper;
using System.Text;

namespace TestSandbox.MessagePacking
{
    public class JsonSerializerAdapter: ISerializerAdapter
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializerAdapter(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <inheritdoc/>
        public byte[] Serialize<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

        /// <inheritdoc/>
        public T Deserialize<T>(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
