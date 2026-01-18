using MessagePack;
using Newtonsoft.Json;
using NLog;
using SymOntoClay.CoreHelper.SerializerAdapters;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.IO;
using System.Xml.Linq;

namespace TestSandbox.MessagePacking
{
    public class MessagePackHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            //_logger.Info($" = {}");

            _logger.Info("End");
        }

        private void Case5()
        {
            var fileName = @"d:\Repos\SymOntoClay\TestSandbox\bin\Debug\net9.0\#020ED339-6313-459A-900D-92F809CEBDC5__1_3_3.soc_msg";

            var data = File.ReadAllBytes(fileName);

            var mpAdapter = SerializerAdapterFactory.Create(KindOfSerialization.MessagePack);

            var restored = mpAdapter.Deserialize<BaseMessage>(data);

            _logger.Info($"restored = {restored}");
        }

        private void Case4()
        {
            var logMessage = new AddEndpointMessage();
            logMessage.DateTimeStamp = DateTime.Now;
            logMessage.NodeId = "E4567D";
            logMessage.ThreadId = "M5890A";
            logMessage.GlobalMessageNumber = 17ul;

            /*
        [Key(4)]
        public ulong MessageNumber { get; set; }

        [Key(5)]
        public string MessagePointId { get; set; }

        [Key(6)]
        public string ClassFullName { get; set; }

        [Key(7)]
        public string MemberName { get; set; }

        [Key(8)]
        public string SourceFilePath { get; set; }

        [Key(9)]
        public int SourceLineNumber { get; set; }
            
                [Key(10)]
        public string EndpointName { get; set; }

        [Key(11)]
        public List<int> ParamsCountList { get; set; }
            */

            _logger.Info($"logMessage = {logMessage}");

            var mpAdapter = SerializerAdapterFactory.Create(KindOfSerialization.MessagePack);
            var jsonAdapter = SerializerAdapterFactory.Create(KindOfSerialization.Json);
            var bsonAdapter = SerializerAdapterFactory.Create(KindOfSerialization.Bson);

            byte[] mpData = mpAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), mpData);

            var restored = MessagePackSerializer.Deserialize<AddEndpointMessage>(mpData);

            _logger.Info($"restored = {restored}");

            var jsonData = jsonAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), jsonData);

            var bsonData = bsonAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), bsonData);
        }

        private void Case3()
        {
            var logMessage = new TstLogMessage();
            logMessage.Timestamp = DateTime.Now;
            logMessage.Message = "Hi!";
            logMessage.SomeField = "DDD";
            logMessage.Level = "16";
            logMessage.Exception = "1235H";

            _logger.Info($"logMessage = {logMessage}");

            var mpAdapter = SerializerAdapterFactory.Create(KindOfSerialization.MessagePack);
            var jsonAdapter = SerializerAdapterFactory.Create(KindOfSerialization.Json);
            var bsonAdapter = SerializerAdapterFactory.Create(KindOfSerialization.Bson);

            byte[] mpData = mpAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), mpData);

            var restored = MessagePackSerializer.Deserialize<TstLogMessage>(mpData);

            _logger.Info($"restored = {restored}");

            var jsonData = jsonAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), jsonData);

            var bsonData = bsonAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), bsonData);
        }

        private void Case2()
        {
            var logMessage = new TstLogMessage();
            logMessage.Timestamp = DateTime.Now;
            logMessage.Message = "Hi!";
            logMessage.SomeField = "DDD";
            logMessage.Level = "16";
            logMessage.Exception = "1235H";

            _logger.Info($"logMessage = {logMessage}");

            var mpAdapter = new MessagePackSerializerAdapter();
            var jsonAdapter = new JsonSerializerAdapter();
            var bsonAdapter = new BsonSerializerAdapter();

            byte[] mpData = mpAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), mpData);

            var restored = MessagePackSerializer.Deserialize<TstLogMessage>(mpData);

            _logger.Info($"restored = {restored}");

            var jsonData = jsonAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), jsonData);

            var bsonData = bsonAdapter.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), bsonData);
        }

        private void Case1()
        {
            var logMessage = new TstLogMessage();
            logMessage.Timestamp = DateTime.Now;
            logMessage.Message = "Hi!";
            logMessage.SomeField = "DDD";

            _logger.Info($"logMessage = {logMessage}");

            byte[] data = MessagePackSerializer.Serialize(logMessage);

            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName()), data);

            var text = JsonConvert.SerializeObject(logMessage);

            _logger.Info($"text = {text}");

            var restored = MessagePackSerializer.Deserialize<TstLogMessage>(data);

            _logger.Info($"restored = {restored}");
        }
    }
}
