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
            var serializedObjectsPool = new SerializedObjectsPool();

            var worlContext = new TstWorldContext();

            var result = serializedObjectsPool.IsSerialized(worlContext);

            _logger.Info($"result = {result}");

            var result2 = serializedObjectsPool.TryGetSerializedValue(worlContext, out var serializedValue);

            _logger.Info($"result2 = {result2}");
            _logger.Info($"serializedValue = {serializedValue}");

            var serializedValue2 = serializedObjectsPool.RegSerializedValue(worlContext);

            _logger.Info($"serializedValue2 = {serializedValue2}");
        }

        private void Case2()
        {
            var serializedTypesPool = new SerializedTypesPool();

            var typeId = serializedTypesPool.GetOrRegisterType(null);

            _logger.Info($"typeId = {typeId}");

            typeId = serializedTypesPool.GetOrRegisterType(typeof(int));

            _logger.Info($"typeId = {typeId}");

            var worlContext = new TstWorldContext();

            typeId = serializedTypesPool.GetOrRegisterType(worlContext?.GetType());

            _logger.Info($"typeId = {typeId}");

            var dict = new Dictionary<int, string>();

            typeId = serializedTypesPool.GetOrRegisterType(dict?.GetType());

            _logger.Info($"typeId = {typeId}");
        }

        private void Case1()
        {
            var value = new SerializedValue(KindOfSerializedValue.ObjectPtr, 1, 1, null);

            _logger.Info($"value.GetHashCode() = {value.GetHashCode()}");
            _logger.Info($"value = {value}");
        }
    }
}
