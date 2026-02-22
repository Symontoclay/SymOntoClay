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

        public MonitorFileWriter(string messagesDir, string sessionName)
        {
#if DEBUG
            _globalLogger.Info($"messagesDir = {messagesDir}");
            _globalLogger.Info($"sessionName = {sessionName}");
#endif

            throw new NotImplementedException("25B3E7BC-5C20-42EC-B2AD-88D1C8C0227C");
        }

        /// <inheritdoc/>
        public string AbsoluteDirectoryName => throw new NotImplementedException("A50156D3-3795-4980-B9E8-A71424CDC9B6");

        /// <inheritdoc/>
        public IMonitorNodeFileWriter CreateMonitorNodeFileWriter(string nodeId)
        {
            throw new NotImplementedException("ADFAE250-9AA6-4219-BDBD-D2A5280F659F");
        }
    }
}
