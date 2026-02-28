using SymOntoClay.Monitor.Common.Data;
using System;

namespace SymOntoClay.Monitor.Internal.FileWriter.General
{
    public class MonitorNodeFileWriter: IMonitorNodeFileWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MonitorNodeFileWriter()
        {
            throw new NotImplementedException("ADFAE250-9AA6-4219-BDBD-D2A5280F659F");
        }

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

            throw new NotImplementedException("F3630A88-60A7-49D2-892A-FB499DD00F6D");
        }
    }
}
