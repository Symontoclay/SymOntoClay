using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestSandbox.SerializationToImage
{
    public class SerializationToImageHandler
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            //Case4();
            //Case3();
            Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case4()
        {
            var worlContext = new TstWorldContext();

            var serializationImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            if (!Directory.Exists(serializationImagesPath))
            {
                Directory.CreateDirectory(serializationImagesPath);
            }

            var serializationPath = Path.Combine(serializationImagesPath, $"Img_{DateTime.Now:yyyyMMdd_HHmmss}.pckg");

            _logger.Info($"serializationPath = {serializationPath}");

            var serializationSettings = new SerializationToImageSettings();
            serializationSettings.ImagePath = serializationPath;

            _logger.Info($"serializationSettings = {serializationSettings}");

            var serializer = new SerializerToImage(serializationSettings);
            serializer.Serialize(worlContext);
        }

        private void Case3()
        {
            var serializedTypesPool = new SerializedTypesPool();

            var serializedObjectsPool = new SerializedObjectsPool(serializedTypesPool);

            NWorkWithSerializedObjectsPool(serializedObjectsPool, null);

            var worlContext = new TstWorldContext();

            NWorkWithSerializedObjectsPool(serializedObjectsPool, worlContext);
        }

        private void NWorkWithSerializedObjectsPool(SerializedObjectsPool serializedObjectsPool, object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var result = serializedObjectsPool.IsSerialized(obj);

            _logger.Info($"result = {result}");

            var result2 = serializedObjectsPool.TryGetSerializedValue(obj, out var serializedValue);

            _logger.Info($"result2 = {result2}");
            _logger.Info($"serializedValue = {serializedValue}");

            var serializedValue2 = serializedObjectsPool.RegSerializedValue(obj);

            _logger.Info($"serializedValue2 = {serializedValue2}");
        }

        private void Case2()
        {
            var serializedTypesPool = new SerializedTypesPool();

            NWorkWithSerializedTypesPool(serializedTypesPool, null);

            NWorkWithSerializedTypesPool(serializedTypesPool, 12);

            var worlContext = new TstWorldContext();

            NWorkWithSerializedTypesPool(serializedTypesPool, worlContext);

            var dict = new Dictionary<int, string>();

            NWorkWithSerializedTypesPool(serializedTypesPool, dict);
        }

        private void NWorkWithSerializedTypesPool(SerializedTypesPool serializedTypesPool, object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            var type = obj?.GetType();

            var typeId = serializedTypesPool.GetOrRegisterType(type);

            _logger.Info($"typeId = {typeId}");

            var kindOfSerializedValue = serializedTypesPool.GetKindOfSerializedValue(type);

            _logger.Info($"kindOfSerializedValue = {kindOfSerializedValue}");
        }

        private void Case1()
        {
            var value = new SerializedValue(KindOfSerializedValue.ObjectPtr, 1, 1, null);

            _logger.Info($"value.GetHashCode() = {value.GetHashCode()}");
            _logger.Info($"value = {value}");
        }
    }
}
