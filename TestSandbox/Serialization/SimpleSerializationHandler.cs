using SymOntoClay.Core;
using SymOntoClay.Serialization.Implementation;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using SymOntoClay.Serialization.SmartValues.Functors;
using SymOntoClay.Serialization.SmartValues;

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

            TstFieldSmartValue();
            //TstPathCombineSmartValue();
            //DeserializeTstInternalWorldSerializableObject();
            //SerializeTstInternalWorldSerializableObject();
            //SimpleDictionarySerialization();
            //Deserialize();
            //Serialize();

            _logger.Info("End");
        }

        private void TstFieldSmartValue()
        {
            var settings = new TstExternalSettings
            {
                Prop1 = "Hi from deserialization"
            };

            var sourceSmartValue = new ConstSmartValue<TstExternalSettings>(settings);

            var propSmartValue = new FieldSmartValue<TstExternalSettings, string>(sourceSmartValue, nameof(settings.Prop1));

#if DEBUG
            _logger.Info($"propSmartValue = {propSmartValue}");
#endif

            var propValue = propSmartValue.Value;

#if DEBUG
            _logger.Info($"propValue = {propValue}");
#endif
        }

        private void TstPathCombineSmartValue()
        {
            var smartValue = new PathCombineSmartValue(new ConstSmartValue<string>(Directory.GetCurrentDirectory()), new ConstSmartValue<string>("SomeDir"));

#if DEBUG
            _logger.Info($"smartValue.Value = {smartValue.Value}");
#endif
        }

        private void DeserializeTstInternalWorldSerializableObject()
        {
            var settings = new TstExternalSettings
            {
                Prop1 = "Hi from deserialization"
            };

#if DEBUG
            _logger.Info($"settings = {settings}");
#endif

            var obj = new TstWorldSerializableObject(settings);

#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var serializationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            obj.Load(new SymOntoClay.Serialization.SerializationSettings()
            {
                Path = serializationDirectory,
                ImageName = "TmpImage"
            });

#if DEBUG
            _logger.Info($"obj (after) = {obj}");
#endif
        }

        private void SerializeTstInternalWorldSerializableObject()
        {
            var settings = new TstExternalSettings
            {
                Prop1 = "Hi from serialization"
            };

#if DEBUG
            _logger.Info($"settings = {settings}");
#endif

            var obj = new TstWorldSerializableObject(settings);

#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            obj.SetSomeValue(16);

#if DEBUG
            _logger.Info($"obj (after) = {obj}");
#endif

            var serializationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            if (!Directory.Exists(serializationDirectory))
            {
                Directory.CreateDirectory(serializationDirectory);
            }

            obj.Save(new SymOntoClay.Serialization.SerializationSettings()
            {
                 Path = serializationDirectory,
                 ImageName = "TmpImage"
            });
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
            //var instance = deserializer.Deserialize<Class1>();
            //var instance = deserializer.Deserialize<Class2>();
            //var instance = deserializer.Deserialize<Class3>();

            _logger.Info($"instance = {instance}");
        }

        private void Serialize()
        {
            var instance = new SerializedObject(true)
            {
                IntField = 16
            };

            //var instance = new Class1();
            //var instance = new Class2(true);
            //var instance = new Class3(true);

            _logger.Info($"instance = {instance}");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "SomeSerializedObject");

            _logger.Info($"path = {path}");

            var serializationContext = new SerializationContext(path);

            var serializer = new Serializer(serializationContext);

            serializer.Serialize(instance);
        }
    }
}
