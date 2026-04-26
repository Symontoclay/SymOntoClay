using SymOntoClay.CoreHelper.SerializerAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializerToImage
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public SerializerToImage(SerializationToImageSettings serializationSettings, IStructuralContext structuralContext)
        {
            _structuralContext = structuralContext;

            _imageFileName = serializationSettings.ImageFileName;

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
            _rootObjectsAndSettingsDataCardWriter = new RootObjectsAndSettingsDataCardWriter();
            _rootObjectsAndSettingsSerializer = new RootObjectsAndSettingsSerializer(_serializedObjectsPool, structuralContext, _rootObjectsAndSettingsDataCardWriter);
            _objectToImageSerializer = new ObjectToImageSerializer(_serializedObjectsPool);
        }

        private readonly string _imageFileName;
        private readonly string _baseTempPath;
        private readonly string _tempPath;
        private readonly IStructuralContext _structuralContext;
        private readonly ISerializedTypesPool _serializedTypesPool;
        private readonly ITypesHelper _typesHelper;
        private readonly ISerializedObjectsPool _serializedObjectsPool;
        private readonly IDataCardWriter _rootObjectsAndSettingsDataCardWriter;
        private readonly IObjectSerializer _rootObjectsAndSettingsSerializer;
        private readonly IObjectSerializer _objectToImageSerializer;

        private List<(string EntryName, string FilePath)> _filesToPack = new List<(string EntryName, string FilePath)>();

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

            var rootSerializedSettingsValue = _rootObjectsAndSettingsSerializer.SerializeValue(obj);

#if DEBUG
            _logger.Info($"rootSerializedSettingsValue = {rootSerializedSettingsValue}");
#endif

            //var rootSerializedValue = _objectToImageSerializer.SerializeValue(obj);

#if DEBUG
            //_logger.Info($"rootSerializedValue = {rootSerializedValue}");
#endif

            //throw new NotImplementedException("C5663A8F-AD33-4C0B-A90A-6E82E64D9D8C");
        }

        private void Finalization()
        {
            SaveSerializedTypesPoolToFile();
            CreatePackage();

            Directory.Delete(_tempPath, true);
        }

        private void SaveSerializedTypesPoolToFile()
        {
            var packEntryName = "Types.dat";

            var fullFileName = Path.Combine(_tempPath, packEntryName);

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            _filesToPack.Add((packEntryName, fullFileName));

            using var fs = new FileStream(fullFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            using var writer = new BinaryWriter(fs);

            _serializedTypesPool.Save(writer);

            fs.Flush();
        }

        private void CreatePackage()
        {
            using var zipToOpen = new FileStream(_imageFileName, FileMode.Create);
            using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);

            foreach (var entry in _filesToPack) 
            {
#if DEBUG
                _logger.Info($"entry = {entry}");
#endif

                archive.CreateEntryFromFile(entry.FilePath, entry.EntryName, CompressionLevel.Optimal);
            }
        }
    }
}
