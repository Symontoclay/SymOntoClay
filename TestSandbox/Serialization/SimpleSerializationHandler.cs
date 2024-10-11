using SymOntoClay.Core;
using SymOntoClay.Serialization.Implementation;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace TestSandbox.Serialization
{
    public class SimpleSerializationHandler
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public void Run()
        {
            _logger.Info("Begin");

            //SimpleDictionarySerialization();
            //Deserialize();
            Serialize();

            _logger.Info("End");
        }

        private void SimpleDictionarySerialization()
        {
            var dict = new Dictionary<object, object>();

            dict["Hi"] = 16;
            dict[5] = "DDD";
            dict[this] = 7;

#if DEBUG
            _logger.Info($"dict = {JsonConvert.SerializeObject(dict, Formatting.Indented)}");
#endif

            var kvpList = dict.ToList();

#if DEBUG
            _logger.Info($"kvpList = {JsonConvert.SerializeObject(kvpList, Formatting.Indented)}");
#endif

            var kvpListJson = JsonConvert.SerializeObject(kvpList, SerializationHelper.JsonSerializerSettings);

#if DEBUG
            _logger.Info($"kvpListJson = {kvpListJson}");
#endif

            var kvpList2 = JsonConvert.DeserializeObject<List<KeyValuePair<object, object>>>(kvpListJson, SerializationHelper.JsonSerializerSettings);

            var dict2 = kvpList2.ToDictionary();

#if DEBUG
            _logger.Info($"dict2 = {JsonConvert.SerializeObject(dict2, Formatting.Indented)}");
#endif
        }

        private void Deserialize()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "SomeSerializedObject");

            _logger.Info($"path = {path}");

            var deserializationContext = new DeserializationContext(path);

            var deserializer = new Deserializer(deserializationContext);

            var instance = deserializer.Deserialize<SerializedObject>();

            _logger.Info($"instance = {instance}");
        }

        private void Serialize()
        {
            var instance = new SerializedObject(true)
            {
                IntField = 16
            };

            _logger.Info($"instance = {instance}");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "SomeSerializedObject");

            _logger.Info($"path = {path}");

            var serializationContext = new SerializationContext(path);

            var serializer = new Serializer(serializationContext);

            serializer.Serialize(instance);
        }
    }
}
