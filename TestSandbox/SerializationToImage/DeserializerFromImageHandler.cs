using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SymOntoClay.CoreHelper.SerializationToImage;
using System.IO;

namespace TestSandbox.SerializationToImage
{
    public class DeserializerFromImageHandler
    {
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case1();

            _logger.Info("End");
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

                var serializer = new JsonSerializer();

                var dataCard = serializer.Deserialize<IDataCard>(bsonReader);

                _logger.Info($"dataCard = {dataCard}");
            }
        }
    }
}
