using System;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializerToImage
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public SerializerToImage(SerializationToImageSettings serializationSettings)
        {
            _baseTempPath = serializationSettings.BaseTempPath;

            if(string.IsNullOrWhiteSpace(_baseTempPath))
            {
                _baseTempPath = Environment.GetEnvironmentVariable("TMP");
            }

#if DEBUG
            _logger.Info($"_baseTempPath = {_baseTempPath}");
#endif

            _tempPath = Path.Combine(_baseTempPath, $"TempImage_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
        }

        private readonly string _baseTempPath;
        private readonly string _tempPath;

        public void Serialize(object obj)
        {
            Preparation();
            Run(obj);
            Finalization();
        }

        private void Preparation()
        {

        }

        private void Run(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            throw new NotImplementedException("C5663A8F-AD33-4C0B-A90A-6E82E64D9D8C");
        }

        private void Finalization()
        {

        }
    }
}
