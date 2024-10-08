using SymOntoClay.Core;
using SymOntoClay.Serialization.Implementation;
using System.IO;
using System;

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

            //Deserialize();
            Serialize();

            _logger.Info("End");
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
