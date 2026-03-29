using SymOntoClay.Common.Disposing;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Formats;
using System;
using System.IO;

namespace SymOntoClay.Monitor.Internal.FileWriter.Binary
{
    public class MonitorFileWriter: Disposable, IMonitorFileWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MonitorFileWriter(string messagesDir, string sessionName)
        {
#if DEBUG
            //_globalLogger.Info($"messagesDir = {messagesDir}");
            //_globalLogger.Info($"sessionName = {sessionName}");
#endif

            _absoluteDirectory = Path.Combine(messagesDir, sessionName);

#if DEBUG
            //_globalLogger.Info($"_absoluteDirectory = {_absoluteDirectory}");
#endif

            if (!Directory.Exists(_absoluteDirectory))
            {
                Directory.CreateDirectory(_absoluteDirectory);
            }

            var dataFileName = Path.Combine(_absoluteDirectory, "Logs.dat");

#if DEBUG
            //_globalLogger.Info($"dataFileName = {dataFileName}");
#endif

            _dataStream = new FileStream(dataFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _dataWriter = new BinaryWriter(_dataStream);
        }

        private readonly object _lock = new object();

        private readonly string _absoluteDirectory;
        private readonly FileStream _dataStream;
        private readonly BinaryWriter _dataWriter;

        /// <inheritdoc/>
        public string AbsoluteDirectoryName => _absoluteDirectory;

        /// <inheritdoc/>
        public IMonitorNodeFileWriter CreateMonitorNodeFileWriter(string nodeId)
        {
#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
#endif

            return new MonitorNodeFileWriter(nodeId, _absoluteDirectory);
        }

        /// <inheritdoc/>
        public void WriteData(string nodeId, string threadId, ulong messageNumber, ulong globalMessageNumber, KindOfMessage kindOfMessage, byte[] data)
        {
#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"threadId = {threadId}");
            //_globalLogger.Info($"messageNumber = {messageNumber}");
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
            //_globalLogger.Info($"data.Length = {data.Length}");
#endif

            lock(_lock)
            {
                MonitorLogFileRowFormat.Write(_dataWriter, nodeId, threadId, messageNumber, globalMessageNumber, (int)kindOfMessage, data);

                _dataStream.Flush();
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
#if DEBUG
            _globalLogger.Info("OnDisposing!!!!!! ");
#endif

            _dataWriter.Dispose();
            _dataStream.Dispose();
            
            base.OnDisposing();
        }
    }
}
