using Newtonsoft.Json;
using SymOntoClay.Common.Disposing;
using SymOntoClay.CoreHelper.SerializerAdapters;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage.DataCardWriters
{
    public class BaseDataCardFileWriter : Disposable, IDataCardWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        protected BaseDataCardFileWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack, string packEntryName)
        {
            _packEntryName = packEntryName;
            _filesToPack = filesToPack;

            var fullFileName = Path.Combine(basePath, packEntryName);

#if DEBUG
            //_logger.Info($"fullFileName = {fullFileName}");
#endif

            _filesToPack.Add((packEntryName, fullFileName));

            _fs = new FileStream(fullFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            _writer = new BinaryWriter(_fs);
        }

        private List<(string EntryName, string FilePath)> _filesToPack;

        private readonly string _packEntryName;
        private readonly Stream _fs;
        private readonly BinaryWriter _writer;
        private readonly BsonSerializerAdapter _bsonSerializerAdapter = new BsonSerializerAdapter();
        private readonly JsonSerializer _serializer = new JsonSerializer();

        /// <inheritdoc/>
        public string RelativePath => _packEntryName;

        /// <inheritdoc/>
        public void Write(IDataCard dataCard)
        {
#if DEBUG
            //_logger.Info($"dataCard = {dataCard}");
            //_logger.Info($"dataCard.KindOfDataCard = {dataCard.KindOfDataCard}");
#endif

            var data = _bsonSerializerAdapter.Serialize(dataCard, _serializer);

#if DEBUG
            //_logger.Info($"data.Length = {data.Length}");
#endif

            _writer.Write((int)dataCard.KindOfDataCard);
            _writer.Write(data.Length);
            _writer.Write(data);

            _fs.Flush();
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            _writer.Dispose();
            _fs.Dispose();

            base.OnDisposing();
        }
    }
}
