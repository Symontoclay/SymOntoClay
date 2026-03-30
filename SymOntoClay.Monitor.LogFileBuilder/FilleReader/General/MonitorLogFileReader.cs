using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            return File.ReadAllBytes(fileName);
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

            foreach (var file in filesList)
            {
#if DEBUG
                _globalLogger.Info($"file = {file}");
#endif

                var item1 = file.Item1;

                if (targetKindOfMessages != null)
                {
                    if(!targetKindOfMessages.Contains(item1.KindOfMessage))
                    {
                        continue;
                    }
                }

                var record = new LogFileRowRecord
                    (
                        NodeId: item1.NodeId,
                        ThreadId: item1.ThreadId,
                        MessageNumber: item1.MessageNumber,
                        GlobalMessageNumber: item1.GlobalMessageNumber,
                        KindOfMessage: item1.KindOfMessage,
                        DataLength: 0,
                        Data: null,
                        FileName: file.p
                    );
            
#if DEBUG
                _globalLogger.Info($"record = {record}");
#endif

                result.Add(record);
            }

#if DEBUG
            //throw new NotImplementedException("C49CCCE9-1C48-48B3-89C0-7ABF9EB6C80E");
#endif

            var subDirectories = Directory.GetDirectories(targetDirectoryName);

            var nextLevelNum = levelNum + 1;

            foreach (var subDirectory in subDirectories)
            {
                FillUpFileRowRecords(ref result, subDirectory, targetKindOfMessages, nextLevelNum, targetNodes, targetThreads);
            }
        }
    }
}
