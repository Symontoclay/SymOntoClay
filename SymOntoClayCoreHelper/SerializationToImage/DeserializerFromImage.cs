using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class DeserializerFromImage
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public DeserializerFromImage(SerializationToImageSettings serializationSettings, IStructuralContext structuralContext)
        {
            _structuralContext = structuralContext;

            _imageFileName = serializationSettings.ImageFileName;

            _baseTempPath = serializationSettings.BaseTempPath;

            if (string.IsNullOrWhiteSpace(_baseTempPath))
            {
                _baseTempPath = Environment.GetEnvironmentVariable("TMP");
            }

#if DEBUG
            //_logger.Info($"_baseTempPath = {_baseTempPath}");
#endif

            _tempPath = Path.Combine(_baseTempPath, $"TempImage_{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}");

            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }

            _serializedTypesPool = new SerializedTypesPool();
            _typesHelper = new TypesHelper();
        }

        private readonly IStructuralContext _structuralContext;
        private readonly string _imageFileName;
        private readonly string _baseTempPath;
        private readonly string _tempPath;
        private ImageManifest _manifest;
        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly ITypesHelper _typesHelper;

        public void Deserialize(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            Preparation();

            throw new NotImplementedException("C072330D-6772-4582-948D-CA412DCD07FE");
        }

        private void Preparation()
        {
            UnPackPackage();
            ReadManifest();
            LoadSerializedTypesPoolFromFile();
        }

        private void UnPackPackage()
        {
#if DEBUG
            _logger.Info($"_imageFileName = {_imageFileName}");
            _logger.Info($"_tempPath = {_tempPath}");
#endif

            ZipFile.ExtractToDirectory(_imageFileName, _tempPath);
        }

        private void ReadManifest()
        {
            var packEntryName = PackEntryNames.Manifest;

            var fullFileName = Path.Combine(_tempPath, packEntryName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            var content = File.ReadAllText(fullFileName);

#if DEBUG
            _logger.Info($"content = {content}");
#endif

            _manifest = JsonConvert.DeserializeObject<ImageManifest>(content);

#if DEBUG
            _logger.Info($"_manifest = {_manifest}");
#endif
        }

        private void LoadSerializedTypesPoolFromFile()
        {
            var packEntryName = PackEntryNames.Types;

            var fullFileName = Path.Combine(_tempPath, packEntryName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            using var fs = new FileStream(fullFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            using var reader = new BinaryReader(fs);

            _serializedTypesPool.Load(reader);
        }
    }
}
