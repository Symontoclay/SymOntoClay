using Newtonsoft.Json;
using System.Text;

namespace TestSandbox.MessagePacking
{
    public class JsonSerializerAdapter: IMessageSerializer
    {
        /// <inheritdoc/>
        public byte[] Serialize<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
