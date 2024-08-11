using Newtonsoft.Json;
//using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Serialization.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Serialization
{
    public class SerializedCodeChunksHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("2083E440-34CD-4B08-936C-E39EECF01AA9", "Begin");

            DeserializeSimplestUsingCodeChunksContext();
            //SerializeSimplestUsingCodeChunksContext();

            _logger.Info("15E55FD5-04A4-4860-9779-2FA7B3989938", "End");
        }

        private void DeserializeSimplestUsingCodeChunksContext()
        {
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "SomeSerializedObject");

            //_logger.Info("609190D6-DAAF-433F-8E17-15AF68B6A78F", $"path = {path}");

            //var deserializationContext = new DeserializationContext(path);

            //var deserializer = new Deserializer(deserializationContext);

            //var codeChunksContext = deserializer.Deserialize<CodeChunksContext>();

            //codeChunksContext.Run();
        }

        private void SerializeSimplestUsingCodeChunksContext()
        {
            //var codeChunksContext = new CodeChunksContext("BEE29B0A-6806-4981-85DE-4711001A7802");

            //codeChunksContext.CreateCodeChunk("852D8948-7DA6-41C9-B6EE-E038D58F0248", (currentCodeChunk) =>
            //{
            //    //_logger.Info("64C869BD-91D0-4CC5-B675-1BF13A9062F3", "Chunk1");
            //});

            //codeChunksContext.CreateCodeChunk("5A33E959-BC01-40B0-82DE-3874E0E31AD7", (currentCodeChunk) =>
            //{
            //    //_logger.Info("3E5FE705-0F2E-45EE-A579-B080F2FBF566", "Chunk2");
            //});

            //codeChunksContext.Run();

            //var path = Path.Combine(Directory.GetCurrentDirectory(), "SomeSerializedObject");

            //_logger.Info("B26F5FCD-49B6-4E03-9A98-96295B597E8D", $"path = {path}");

            //var serializationContext = new SerializationContext(path);

            //var serializer = new Serializer(serializationContext);

            //serializer.Serialize(codeChunksContext);
        }
    }
}
