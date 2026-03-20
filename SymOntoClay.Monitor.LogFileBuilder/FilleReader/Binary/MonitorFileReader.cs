using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary
{
    public class MonitorFileReader: IFileReader
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private const int _nodeLevel = 2;

        /// <inheritdoc/>
        public List<IndexFileRowRecord> GetIndexFileRowRecords(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            _globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            var result = new List<IndexFileRowRecord>();

            FillUpFileRowRecords(ref result, targetDirectoryName, targetKindOfMessages, 1, targetNodes, targetThreads);

            throw new NotImplementedException("353BDAF9-AF2A-4C24-80CF-9DA637C46DCB");
        }

        private static void FillUpFileRowRecords(ref List<IndexFileRowRecord> result, string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, int levelNum, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            _globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
            _globalLogger.Info($"levelNum = {levelNum}");
            _globalLogger.Info($"targetNodes?.Count() = {targetNodes?.Count()}");
#endif

            switch (levelNum)
            {
                case _nodeLevel:
                    if (targetNodes != null)
                    {
                        var directoryInfo = new DirectoryInfo(targetDirectoryName);

#if DEBUG
                        _globalLogger.Info($"directoryInfo.Name = {directoryInfo.Name}");
#endif

                        if (!targetNodes.Contains(directoryInfo.Name))
                        {
                            return;
                        }
                    }
                    break;

                default:
                    break;
            }

            var logFileName = Path.Combine(targetDirectoryName, "Logs.dat");

#if DEBUG
            _globalLogger.Info($"logFileName = {logFileName}");
            _globalLogger.Info($"File.Exists(logFileName) = {File.Exists(logFileName)}");
#endif

            var indexFileName = Path.Combine(targetDirectoryName, "Logs.idx");

#if DEBUG
            _globalLogger.Info($"indexFileName = {indexFileName}");
            _globalLogger.Info($"File.Exists(indexFileName) = {File.Exists(indexFileName)}");
#endif

            throw new NotImplementedException("2893FD89-655C-4749-B8EB-A4CFF2D083E0");
        }
    }
}
