using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using System;
using System.IO;
using System.Linq;

namespace TestSandbox.SerializationToImage
{
    public class DeserializerFromImageHandler
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case5()
        {
            var fullFileName = Path.Combine(Directory.GetCurrentDirectory(), "SerializationToImage", "Objects.dat");

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            using var dataCardDictionary = new DataCardDictionary(fullFileName);
        }

        private void Case4()
        {
            var fullFileName = Path.Combine(Directory.GetCurrentDirectory(), "SerializationToImage", "Objects.dat");

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            using var dataCardReader = new DataCardReader(fullFileName);

            var cards = dataCardReader.ReadAll().Cast<IDataCardWithHeader>();

#if DEBUG
            _logger.Info($"cards.Count() = {cards.Count()}");
#endif

            foreach (var card in cards)
            {
#if DEBUG
                _logger.Info($"card = {card}");
#endif
            }

            var cardsDict = cards.ToDictionary(p => p.Header, p => p);

#if DEBUG
            _logger.Info($"cardsDict.Count = {cardsDict.Count}");
#endif
        }

        private void Case3()
        {
            var fullFileName = Path.Combine(Directory.GetCurrentDirectory(), "SerializationToImage", "RootObjects.dat");

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            using var dataCardReader = new DataCardReader(fullFileName);

            var cards = dataCardReader.ReadAll();

#if DEBUG
            _logger.Info($"cards.Count = {cards.Count}");
#endif

            foreach (var card in cards) 
            {
#if DEBUG
                _logger.Info($"card = {card}");
#endif
            }
        }

        private void Case2()
        {
            var serializedValue = new SerializedValue(KindOfSerializedValue.Preregistered, 1, 1, "");

#if DEBUG
            _logger.Info($"serializedValue = {serializedValue}");
#endif

            var dataCard = new ExternalWorldComponentClassCard()
            {
                Header = serializedValue,
                Id = "#020ED339-6313-459A-900D-92F809CEBDC5"
            };

#if DEBUG
            _logger.Info($"dataCard = {dataCard}");
#endif

            using var ms = new MemoryStream();
            using var bsonWriter = new BsonDataWriter(ms);

            var serializer = new JsonSerializer();
            serializer.Serialize(bsonWriter, dataCard);

            var data = ms.ToArray();

#if DEBUG
            _logger.Info($"data.Length = {data.Length}");
#endif

            using var ms1 = new MemoryStream(data);
            using var bsonReader = new BsonDataReader(ms1);

            var card = DeserializeDataCard(dataCard.KindOfDataCard, serializer, bsonReader);

            _logger.Info($"card = {card}");
        }

        private void Case1()
        {
            var fullFileName = Path.Combine(Directory.GetCurrentDirectory(), "SerializationToImage", "RootObjects.dat");

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            using var fs = new FileStream(fullFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            using var reader = new BinaryReader(fs);

            _logger.Info($"fs.Length = {fs.Length}");

            var serializer = new JsonSerializer();

            while (fs.Position < fs.Length)
            {
                _logger.Info($"fs.Position = {fs.Position}");

                var kindOfDataCard = reader.ReadInt32();

                _logger.Info($"kindOfDataCard = {kindOfDataCard}");

                var dataLength = reader.ReadInt32();

                _logger.Info($"dataLength = {dataLength}");

                var data = reader.ReadBytes(dataLength);

                _logger.Info($"data.Length = {data.Length}");

                using var ms = new MemoryStream(data);
                using var bsonReader = new BsonDataReader(ms);

                var dataCard = DeserializeDataCard((KindOfDataCard)kindOfDataCard, serializer, bsonReader);

                _logger.Info($"dataCard = {dataCard}");
            }
        }

        private IDataCard DeserializeDataCard(KindOfDataCard kindOfDataCard, JsonSerializer serializer, BsonDataReader bsonReader)
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfDataCard), kindOfDataCard, "7484D7DE-3409-40DC-A57E-2F1E912243E6");
            }
        }
    }
}
