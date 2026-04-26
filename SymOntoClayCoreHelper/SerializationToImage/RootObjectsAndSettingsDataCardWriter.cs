using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class RootObjectsAndSettingsDataCardWriter: IDataCardWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public RootObjectsAndSettingsDataCardWriter(string basePath)
        {
            _basePath = basePath;
        }

        private readonly string _basePath;

        /// <inheritdoc/>
        public void Write(KindOfDataCard kindOfDataCard, object dataCard)
        {
#if DEBUG
            _logger.Info($"kindOfDataCard = {kindOfDataCard}");
#endif

            using var ms = new MemoryStream();
            using var bsonWriter = new BsonDataWriter(ms);

            var serializer = new JsonSerializer();
            serializer.Serialize(bsonWriter, dataCard);

            var data = ms.ToArray();

#if DEBUG
            _logger.Info($"data.Length = {data.Length}");
#endif

            var record = new DataCardRecord(
                    KindOfDataCard: (int)kindOfDataCard,
                    DataLength: data.Length,
                    Data: data
                );

            throw new NotImplementedException();
        }
    }
}
