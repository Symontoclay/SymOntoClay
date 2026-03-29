using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.IO;
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
        private const int _threadLevel = 3;

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

            switch (levelNum)
            {
                case _nodeLevel:
                    if (targetNodes != null)
                    {
                        var directoryInfo = new DirectoryInfo(targetDirectoryName);

#if DEBUG
                        //_globalLogger.Info($"directoryInfo.Name = {directoryInfo.Name}");
#endif

                        if (!targetNodes.Contains(directoryInfo.Name))
                        {
                            return;
                        }
                    }
                    break;

                case _threadLevel:
                    if (targetThreads != null)
                    {
                        var directoryInfo = new DirectoryInfo(targetDirectoryName);

#if DEBUG
                        //_globalLogger.Info($"directoryInfo.Name = {directoryInfo.Name}");
#endif

                        if (!targetThreads.Contains(directoryInfo.Name))
                        {
                            return;
                        }
                    }
                    break;

                default:
                    break;
            }

            var filesList = Directory.GetFiles(targetDirectoryName).Where(p => p.EndsWith(FileCacheItemInfo.FileExt)).Select(p => (FileCacheItemInfo.GetFileInfo(p), p));

            if (targetKindOfMessages?.Any() ?? false)
            {
                filesList = filesList.Where(p => targetKindOfMessages != null && targetKindOfMessages.Contains(p.Item1.KindOfMessage));
            }

            result.AddRange(filesList);

            var subDirectories = Directory.GetDirectories(targetDirectoryName);

            var nextLevelNum = levelNum + 1;

            foreach (var subDirectory in subDirectories)
            {
                FillUpFileRowRecords(ref result, subDirectory, targetKindOfMessages, nextLevelNum, targetNodes, targetThreads);
            }
        }
    }
}
