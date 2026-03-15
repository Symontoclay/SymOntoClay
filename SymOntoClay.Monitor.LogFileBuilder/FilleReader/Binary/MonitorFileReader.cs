using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary
{
    public class MonitorFileReader: IFileReader
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public List<IndexFileRowRecord> GetIndexFileRowRecords(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            _globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            var result = new List<IndexFileRowRecord>();

            throw new NotImplementedException("353BDAF9-AF2A-4C24-80CF-9DA637C46DCB");
        }

        private static void FillUpFileRowRecords(ref List<IndexFileRowRecord> result, string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, int levelNum, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            _globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            throw new NotImplementedException("2893FD89-655C-4749-B8EB-A4CFF2D083E0");
        }
    }
}
