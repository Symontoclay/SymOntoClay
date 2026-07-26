using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.Common.Disposing;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class DataCardReader : Disposable, IDataCardReader
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public DataCardReader(string basePath, string packEntryName)
            : this(Path.Combine(basePath, packEntryName))
        {
        }

        public DataCardReader(string fullFileName)
        {
#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            _fs = new FileStream(fullFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            _reader = new BinaryReader(_fs);
        }

        private readonly Stream _fs;
        private readonly BinaryReader _reader;
        private readonly JsonSerializer _serializer = new JsonSerializer();

        /// <inheritdoc/>
        public List<IDataCard> ReadAll()
        {
#if DEBUG
            //_logger.Info($"_fs.Length = {_fs.Length}");
            //_logger.Info($"_fs.Position = {_fs.Position}");
#endif

            _fs.Position = 0;

            var result = new List<IDataCard>();

            while (_fs.Position < _fs.Length)
            {
#if DEBUG
                //_logger.Info($"_fs.Position = {_fs.Position}");
#endif

                var kindOfDataCard = _reader.ReadInt32();

                //_logger.Info($"kindOfDataCard = {kindOfDataCard}");

                var dataLength = _reader.ReadInt32();

                //_logger.Info($"dataLength = {dataLength}");

                var data = _reader.ReadBytes(dataLength);

                //_logger.Info($"data.Length = {data.Length}");

                using var ms = new MemoryStream(data);
                using var bsonReader = new BsonDataReader(ms);

                var dataCard = DeserializeDataCard((KindOfDataCard)kindOfDataCard, _serializer, bsonReader);

                //_logger.Info($"dataCard = {dataCard}");

                result.Add(dataCard);
            }

            return result;
        }

        public static IDataCard DeserializeDataCard(KindOfDataCard kindOfDataCard, JsonSerializer serializer, BsonDataReader bsonReader)
        {
            switch (kindOfDataCard)
            {
                case KindOfDataCard.ImageRootCard:
                    return serializer.Deserialize<ImageRootCard>(bsonReader);

                case KindOfDataCard.ExternalClassCard:
                    return serializer.Deserialize<ExternalClassCard>(bsonReader);

                case KindOfDataCard.ExternalManualResetEventClassCard:
                    return serializer.Deserialize<ExternalManualResetEventClassCard>(bsonReader);

                case KindOfDataCard.ExternalListCard:
                    return serializer.Deserialize<ExternalListCard>(bsonReader);

                case KindOfDataCard.ExternalWorldRootClassCard:
                    return serializer.Deserialize<ExternalWorldRootClassCard>(bsonReader);

                case KindOfDataCard.ExternalWorldComponentClassCard:
                    return serializer.Deserialize<ExternalWorldComponentClassCard>(bsonReader);

                case KindOfDataCard.KeyWorldComponentClassCard:
                    return serializer.Deserialize<KeyWorldComponentClassCard>(bsonReader);

                case KindOfDataCard.ClassCard:
                    return serializer.Deserialize<ClassCard>(bsonReader);

                case KindOfDataCard.ClassCardWithSerializationData:
                    return serializer.Deserialize<ClassCardWithSerializationData>(bsonReader);

                case KindOfDataCard.ArrayCard:
                    return serializer.Deserialize<ArrayCard>(bsonReader);

                case KindOfDataCard.ListCard:
                    return serializer.Deserialize<ListCard>(bsonReader);

                case KindOfDataCard.HashSetCard:
                    return serializer.Deserialize<HashSetCard>(bsonReader);

                case KindOfDataCard.StackCard:
                    return serializer.Deserialize<StackCard>(bsonReader);

                case KindOfDataCard.QueueCard:
                    return serializer.Deserialize<QueueCard>(bsonReader);

                case KindOfDataCard.DictionaryCard:
                    return serializer.Deserialize<DictionaryCard>(bsonReader);

                case KindOfDataCard.ReflectionTypeCard:
                    return serializer.Deserialize<ReflectionTypeCard>(bsonReader);

                case KindOfDataCard.ActionCard:
                    return serializer.Deserialize<ActionCard>(bsonReader);

                case KindOfDataCard.ExternalValueCard:
                    return serializer.Deserialize<ExternalValueCard>(bsonReader);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfDataCard), kindOfDataCard, "3EA4C344-7D39-47AD-B3D3-8F29FCE3C105");
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            _reader.Dispose();
            _fs.Dispose();

            base.OnDisposing();
        }
    }
}
