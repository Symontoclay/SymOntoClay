using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.CoreHelper.SerializationToImage;
using SymOntoClay.CoreHelper.SerializationToImage.DataCardReaders;
using SymOntoClay.CoreHelper.SerializationToImage.DataCards;
using SymOntoClay.CoreHelper.SerializerAdapters;
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

            Case8();
            //Case7();
            //Case6();
            //Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("End");
        }

        private void Case8()
        {
            var pathString = @"@/_settings/WorldThreadingSettings/CodeExecution";

#if DEBUG
            _logger.Info($"pathString = {pathString}");
#endif

            var path = SerializedObjectPath.Parse(pathString);

#if DEBUG
            _logger.Info($"path = {path}");
#endif
        }

        private void Case7()
        {
            var fullFileName = Path.Combine(Directory.GetCurrentDirectory(), "SerializationToImage", "RootObjectsAndSettings.dat");

#if DEBUG
            _logger.Info($"fullFileName = {fullFileName}");
#endif

            using var dataCardReader = new DataCardReader(fullFileName);

            var cards = dataCardReader.ReadAll().Cast<IDataCardWithPath>();

#if DEBUG
            _logger.Info($"cards.Count() = {cards.Count()}");
#endif

            foreach (var card in cards)
            {
#if DEBUG
                _logger.Info($"card = {card}");
#endif
            }
        }

        private void Case6()
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

            var bsonSerializerAdapter = new BsonSerializerAdapter();
            var serializer = new JsonSerializer();
            
            var data = bsonSerializerAdapter.Serialize(dataCard, serializer);

#if DEBUG
            _logger.Info($"data.Length = {data.Length}");
#endif

            using var ms1 = new MemoryStream(data);
            using var bsonReader = new BsonDataReader(ms1);

            var card = DataCardReader.DeserializeDataCard(dataCard.KindOfDataCard, serializer, bsonReader);

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

                var dataCard = DataCardReader.DeserializeDataCard((KindOfDataCard)kindOfDataCard, serializer, bsonReader);

                _logger.Info($"dataCard = {dataCard}");
            }
        }
    }
}
