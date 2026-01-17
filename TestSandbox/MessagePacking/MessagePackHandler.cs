using MessagePack;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestSandbox.MessagePacking
{
    public class MessagePackHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            Case2();
            //Case1();

            //_logger.Info($" = {}");

            _logger.Info("End");
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
