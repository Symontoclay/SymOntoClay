using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;

namespace SymOntoClay.Monitor.Internal.FileWriter.General
{
    public class MonitorNodeFileWriter: IMonitorNodeFileWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MonitorNodeFileWriter(string nodeId, MonitorFileCache parentMonitorFileCache)
        {
            _fileCache = parentMonitorFileCache.CreateMonitorNodeFileCache(nodeId);
        }

        private readonly MonitorNodeFileCache _fileCache;

        /// <inheritdoc/>
        public IThreadLoggerFileWriter CreateThreadLoggerFileWriter(string theadId)
        {
            throw new NotImplementedException("788EBDEA-B68B-43CA-9D18-6C128513B610");
        }

        /// <inheritdoc/>
        public void WriteData(string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, KindOfMessage kindOfMessage, byte[] data)
        {
#if DEBUG
            _globalLogger.Info($"nodeId = {nodeId}");
            _globalLogger.Info($"threadId = {threadId}");
            _globalLogger.Info($"messageNumber = {messageNumber}");
            _globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
            _globalLogger.Info($"kindOfMessage = {kindOfMessage}");
            _globalLogger.Info($"data.Length = {data.Length}");
#endif

            var fileName = FileCacheItemInfo.GetFileName(nodeId, threadId, messageNumber, globalMessageNumber, kindOfMessage);

#if DEBUG
            _globalLogger.Info($"fileName = {fileName}");
#endif

            _fileCache.WriteFile(fileName, data);
        }
    }
}
