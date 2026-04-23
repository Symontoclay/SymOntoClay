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

            _serializedTypesPool = new SerializedTypesPool();
            _typesHelper = new TypesHelper();
            _serializedObjectsPool = new SerializedObjectsPool(_serializedTypesPool, _typesHelper);
        }

        private readonly string _baseTempPath;
        private readonly string _tempPath;
        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly ITypesHelper _typesHelper;
        private readonly ISerializedObjectsPool _serializedObjectsPool;

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

            var rootSerializedValue = GetSerializedObjectPtr(obj);

#if DEBUG
            _logger.Info($"rootSerializedValue = {rootSerializedValue}");
#endif

            throw new NotImplementedException("C5663A8F-AD33-4C0B-A90A-6E82E64D9D8C");
        }

        private SerializedValue GetSerializedObjectPtr(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            if(_serializedObjectsPool.TryGetSerializedValue(obj, out var serializedValue))
            {
                return serializedValue;
            }

            throw new NotImplementedException("C5394237-52FF-4CDB-89ED-5F3FEA383FD0");
        }

        private void Finalization()
        {
            SaveSerializedTypesPoolToFile();
        }

        private void SaveSerializedTypesPoolToFile()
        {
            throw new NotImplementedException("C325153A-8795-49EA-8382-929223C49CE7");
        }
    }
}
