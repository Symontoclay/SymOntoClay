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

namespace TestSandbox.MessagePacking
{
    public class MessagePackHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

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

            //_logger.Info($" = {}");

            _logger.Info("End");
        }
    }
}
