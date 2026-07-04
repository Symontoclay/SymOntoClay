using SymOntoClay.BaseTestLib;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using TestSandbox.Helpers;

namespace TestSandbox.SerializationToImage
{
    public class SerializationToImageHandler
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            //Case5();
            Case4();
            //Case3_b();
            //Case3_a();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case5()
        {
            //var worldSettings = CreateEmptyWorldSetting();
            var worldSettings = CreateWorldSetting();

#if DEBUG
            _logger.Info($"worldSettings = {worldSettings}");
#endif

            var worlContext = new TstWorldContext(worldSettings);

#if DEBUG
            _logger.Info($"worlContext = {worlContext}");
#endif

            var serializationImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            if (!Directory.Exists(serializationImagesPath))
            {
                Directory.CreateDirectory(serializationImagesPath);
            }

            var serializationPath = Path.Combine(serializationImagesPath, $"Img_{DateTime.Now:yyyyMMdd_HHmmss}.pckg");

            _logger.Info($"serializationPath = {serializationPath}");

            var baseTempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

            if (!Directory.Exists(baseTempPath))
            {
                Directory.CreateDirectory(baseTempPath);
            }

            var serializationSettings = new SerializationToImageSettings();
            serializationSettings.ImageFileName = serializationPath;
            serializationSettings.BaseTempPath = baseTempPath;

            _logger.Info($"serializationSettings = {serializationSettings}");

            var structuralContext = new WorldStructuralContext();

            var serializer = new SerializerToImage(serializationSettings, structuralContext);
            serializer.Serialize(worlContext);
        }

        private void Case4()
        {
            var autoResetEvent = new ManualResetEvent(true);

            var type = autoResetEvent.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
#endif

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

#if DEBUG
            _logger.Info($"fields.Length = {fields.Length}");
#endif

            foreach (var field in fields) 
            {
#if DEBUG
                _logger.Info($"field.Name = {field.Name}");
#endif
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

#if DEBUG
            _logger.Info($"properties.Length = {properties.Length}");
#endif

            foreach (var property in properties) 
            {
#if DEBUG
                _logger.Info($"property.Name = {property.Name}");
#endif
            }

            var isSet = autoResetEvent.WaitOne(0);

#if DEBUG
            _logger.Info($"isSet = {isSet}");
#endif
        }

        private void Case3_b()
        {
            var serializedTypesPool = new SerializedTypesPool();

            var typesHelper = new TypesHelper();

            var serializedObjectsPool = new SerializedObjectsPool(serializedTypesPool, typesHelper);

            var worldSettings = CreateEmptyWorldSetting();

            var worlContext = new TstWorldContext(worldSettings);

            var result = serializedObjectsPool.IsSerialized(worlContext, false);

            _logger.Info($"result = {result}");

            var serializedValue2 = serializedObjectsPool.RegSerializedValue(worlContext, SerializedObjectsPoolMode.General);

            _logger.Info($"serializedValue2 = {serializedValue2}");

            var result2 = serializedObjectsPool.IsSerialized(worlContext, false);

            _logger.Info($"result2 = {result2}");
        }

        private void Case3_a()
        {
            var serializedTypesPool = new SerializedTypesPool();

            var typesHelper = new TypesHelper();

            var serializedObjectsPool = new SerializedObjectsPool(serializedTypesPool, typesHelper);

            NWorkWithSerializedObjectsPoolPreregistered(serializedObjectsPool, null);

            var worldSettings = CreateEmptyWorldSetting();

            var worlContext = new TstWorldContext(worldSettings);

            NWorkWithSerializedObjectsPoolPreregistered(serializedObjectsPool, worlContext);

            NWorkWithSerializedObjectsPoolPreregistered(serializedObjectsPool, 16);
        }

        private void NWorkWithSerializedObjectsPoolPreregistered(SerializedObjectsPool serializedObjectsPool, object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var result = serializedObjectsPool.IsSerialized(obj, true);

            _logger.Info($"result = {result}");

            var result2 = serializedObjectsPool.TryGetSerializedValue(obj, out var serializedValue);

            _logger.Info($"result2 = {result2}");
            _logger.Info($"serializedValue = {serializedValue}");

            var serializedValue2 = serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.IsPreregistered);

            _logger.Info($"serializedValue2 = {serializedValue2}");
        }

        private void Case3()
        {
            var serializedTypesPool = new SerializedTypesPool();

            var typesHelper = new TypesHelper();

            var serializedObjectsPool = new SerializedObjectsPool(serializedTypesPool, typesHelper);

            NWorkWithSerializedObjectsPool(serializedObjectsPool, null);

            var worldSettings = CreateEmptyWorldSetting();

            var worlContext = new TstWorldContext(worldSettings);

            NWorkWithSerializedObjectsPool(serializedObjectsPool, worlContext);

            NWorkWithSerializedObjectsPool(serializedObjectsPool, worlContext);

            NWorkWithSerializedObjectsPool(serializedObjectsPool, 16);
        }

        private void NWorkWithSerializedObjectsPool(SerializedObjectsPool serializedObjectsPool, object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var result = serializedObjectsPool.IsSerialized(obj, false);

            _logger.Info($"result = {result}");

            var result2 = serializedObjectsPool.TryGetSerializedValue(obj, out var serializedValue);

            _logger.Info($"result2 = {result2}");
            _logger.Info($"serializedValue = {serializedValue}");

            var serializedValue2 = serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);

            _logger.Info($"serializedValue2 = {serializedValue2}");
        }

        private void Case2()
        {
            var serializedTypesPool = new SerializedTypesPool();

            NWorkWithSerializedTypesPool(serializedTypesPool, null);

            NWorkWithSerializedTypesPool(serializedTypesPool, 12);

            var dict = new Dictionary<int, string>();

            NWorkWithSerializedTypesPool(serializedTypesPool, dict);

            var list = new List<string>();

            NWorkWithSerializedTypesPool(serializedTypesPool, list);

            var worldSettings = CreateEmptyWorldSetting();

            var worlContext = new TstWorldContext(worldSettings);

            NWorkWithSerializedTypesPool(serializedTypesPool, worlContext);

            var dataFileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName());

#if DEBUG
            _logger.Info($"dataFileName = {dataFileName}");
#endif

            SaveSerializedTypesPoolToFile(dataFileName, serializedTypesPool);

            var serializedTypesPool2 = new SerializedTypesPool();

            LoadSerializedTypesPoolFromFile(dataFileName, serializedTypesPool2);

            var typeId = serializedTypesPool2.GetOrRegisterType(worlContext.GetType());

            _logger.Info($"typeId = {typeId}");
        }

        private void LoadSerializedTypesPoolFromFile(string fileName, SerializedTypesPool serializedTypesPool)
        {
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            using var reader = new BinaryReader(fs);

            serializedTypesPool.Load(reader);
        }

        private void SaveSerializedTypesPoolToFile(string fileName, SerializedTypesPool serializedTypesPool)
        {
            using var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            using var writer = new BinaryWriter(fs);

            serializedTypesPool.Save(writer);

            fs.Flush();
        }

        private void NWorkWithSerializedTypesPool(SerializedTypesPool serializedTypesPool, object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var type = obj?.GetType();

            var typeId = serializedTypesPool.GetOrRegisterType(type);

            _logger.Info($"typeId = {typeId}");

            var typesHelper = new TypesHelper();

            var kindOfSerializedValue = typesHelper.GetKindOfSerializedValue(type);

            _logger.Info($"kindOfSerializedValue = {kindOfSerializedValue}");
        }

        private void Case1()
        {
            var value = new SerializedValue(KindOfSerializedValue.ObjectPtr, 1, 1, null);

            _logger.Info($"value.GetHashCode() = {value.GetHashCode()}");
            _logger.Info($"value = {value}");
        }

        private WorldSettings CreateEmptyWorldSetting()
        {
            var settings = new WorldSettings();

            return settings;
        }

        private WorldSettings CreateWorldSetting()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = this;

            factorySettings.Categories = new List<string>() { "elf" };
            factorySettings.EnableCategories = true;

            var settings = TstEngineContextHelper.CreateWorldSettings(factorySettings);

            _logger.Info($"settings = {settings}");

            return settings;
        }
    }
}
