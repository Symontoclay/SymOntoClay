using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader.General
{
    public class MonitorLogFileReader : ILogFileReader
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private const int _nodeLevel = 2;

        /// <inheritdoc/>
        public List<LogFileRowRecord> GetIndexFileRowRecords(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            _globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            var result = new List<LogFileRowRecord>();

            FillUpFileRowRecords(ref result, targetDirectoryName, targetKindOfMessages, 1, targetNodes, targetThreads);

            return result;
        }

        /// <inheritdoc/>
        public byte[] ReadData(string fileName, byte[] data)
        {
#if DEBUG
            _globalLogger.Info($"fileName = {fileName}");
#endif

            throw new NotImplementedException("C44CC7CF-7C8C-444D-8B96-9D270C8B6B57");
        }

        private void FillUpFileRowRecords(ref List<LogFileRowRecord> result, string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, int levelNum, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            _globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
            _globalLogger.Info($"levelNum = {levelNum}");
            _globalLogger.Info($"targetNodes?.Count() = {targetNodes?.Count()}");
#endif

            throw new NotImplementedException("C3D2704F-B0C7-4131-8F19-D1876C8A85FC");
        }
    }
}
