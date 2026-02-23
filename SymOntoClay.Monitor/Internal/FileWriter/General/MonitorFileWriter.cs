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
            return new MonitorNodeFileWriter();
        }
    }
}
