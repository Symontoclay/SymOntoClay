using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Internal.FileWriter.General
{
    public class MonitorFileWriter : IMonitorFileWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly MonitorFileCache _fileCache;

        public MonitorFileWriter(string messagesDir, string sessionName)
        {
#if DEBUG
            _globalLogger.Info($"messagesDir = {messagesDir}");
            _globalLogger.Info($"sessionName = {sessionName}");
#endif

            _fileCache = new MonitorFileCache(messagesDir, sessionName);
        }

        /// <inheritdoc/>
        public string AbsoluteDirectoryName => _fileCache.AbsoluteDirectoryName;

        /// <inheritdoc/>
        public IMonitorNodeFileWriter CreateMonitorNodeFileWriter(string nodeId)
        {
            return new MonitorNodeFileWriter(nodeId, _fileCache);
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
