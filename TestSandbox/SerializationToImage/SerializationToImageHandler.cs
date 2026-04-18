using SymOntoClay.CoreHelper.SerializationToImage;
using System;
using System.IO;

namespace TestSandbox.SerializationToImage
{
    public class SerializationToImageHandler
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            //Case2();
            Case1();

            _logger.Info("End");
        }

        private void Case2()
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

        private void Case1()
        {
            var value = new SerializedValue(KindOfSerializedValue.ObjectPtr, 1, 1, null);

            _logger.Info($"value.GetHashCode() = {value.GetHashCode()}");
            _logger.Info($"value = {value}");
        }
    }
}
