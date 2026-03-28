using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Threading;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary
{
    public class MonitorIndexFileReader: IIndexFileReader
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private const int _nodeLevel = 2;

        /// <inheritdoc/>
        public List<IndexFileRowRecord> GetIndexFileRowRecords(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            //_globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
#endif

            var result = new List<IndexFileRowRecord>();

            FillUpFileRowRecords(ref result, targetDirectoryName, targetKindOfMessages, 1, targetNodes, targetThreads);

            return result;
        }

        private void FillUpFileRowRecords(ref List<IndexFileRowRecord> result, string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, int levelNum, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads)
        {
#if DEBUG
            //_globalLogger.Info($"targetDirectoryName = {targetDirectoryName}");
            //_globalLogger.Info($"levelNum = {levelNum}");
            //_globalLogger.Info($"targetNodes?.Count() = {targetNodes?.Count()}");
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

                default:
                    break;
            }

            var logFileName = Path.Combine(targetDirectoryName, "Logs.dat");

#if DEBUG
            //_globalLogger.Info($"logFileName = {logFileName}");
            //_globalLogger.Info($"File.Exists(logFileName) = {File.Exists(logFileName)}");
#endif

            var indexFileName = Path.Combine(targetDirectoryName, "Logs.idx");

#if DEBUG
            _globalLogger.Info($"indexFileName = {indexFileName}");
            _globalLogger.Info($"File.Exists(indexFileName) = {File.Exists(indexFileName)}");
#endif

            if(File.Exists(logFileName) && File.Exists(indexFileName))
            {
                var recordsList = ReadIndexFile(indexFileName, logFileName);

#if DEBUG
                //_globalLogger.Info($"recordsList.Count = {recordsList.Count}");
                //_globalLogger.Info($"recordsList = {recordsList.WritePODListToString()}");
#endif

                result.AddRange(recordsList);
            }

            var subDirectories = Directory.GetDirectories(targetDirectoryName);

            var nextLevelNum = levelNum + 1;

            foreach (var subDirectory in subDirectories)
            {
                FillUpFileRowRecords(ref result, subDirectory, targetKindOfMessages, nextLevelNum, targetNodes, targetThreads);
            }
        }

        private List<IndexFileRowRecord> ReadIndexFile(string indexFileName, string logFileName)
        {
#if DEBUG
            //_globalLogger.Info($"indexFileName = {indexFileName}");
            //_globalLogger.Info($"logFileName = {logFileName}");
#endif

            var records = new List<IndexFileRowRecord>();

            using var idxFs = new FileStream(indexFileName, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(idxFs);

            while (idxFs.Position < idxFs.Length)
            {
                var idxRecord = MonitorLogFileRowFormat.Read(reader);

#if DEBUG
                //_globalLogger.Info($"idxRecord = {idxRecord}");
#endif

                var record = new IndexFileRowRecord(
                        NodeId: idxRecord.NodeId,
                        ThreadId: idxRecord.ThreadId,
                        MessageNumber: idxRecord.MessageNumber,
                        GlobalMessageNumber: idxRecord.GlobalMessageNumber,
                        KindOfMessage: (KindOfMessage)idxRecord.KindOfMessage,
                        StartPosition: idxRecord.StartPosition,
                        DataLength: idxRecord.DataLength,
                        FileName: logFileName
                    );

#if DEBUG
                //_globalLogger.Info($"record = {record}");
#endif

                records.Add(record);
            }

            return records;
        }
    }
}
