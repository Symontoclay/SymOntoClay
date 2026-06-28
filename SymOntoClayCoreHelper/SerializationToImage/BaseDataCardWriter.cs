using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.Common.Disposing;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class BaseDataCardWriter : Disposable, IDataCardWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        protected BaseDataCardWriter(string basePath, List<(string EntryName, string FilePath)> filesToPack, string packEntryName)
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

        /// <inheritdoc/>
        public string RelativePath => _packEntryName;

        /// <inheritdoc/>
        public void Write(IDataCard dataCard)
        {
#if DEBUG
            //_logger.Info($"dataCard.KindOfDataCard = {dataCard.KindOfDataCard}");
#endif

            using var ms = new MemoryStream();
            using var bsonWriter = new BsonDataWriter(ms);

            var serializer = new JsonSerializer();
            serializer.Serialize(bsonWriter, dataCard);

            var data = ms.ToArray();

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
