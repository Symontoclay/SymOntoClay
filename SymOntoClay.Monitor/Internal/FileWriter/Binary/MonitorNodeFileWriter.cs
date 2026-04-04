using SymOntoClay.Common.Disposing;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.Monitor.Internal.FileWriter.Binary
{
    public class MonitorNodeFileWriter : Disposable, IMonitorNodeFileWriter, IThreadLoggerFileWriter
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MonitorNodeFileWriter(string nodeId, string absoluteDirectory)
        {
#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
            //_globalLogger.Info($"absoluteDirectory = {absoluteDirectory}");
#endif

            _absoluteDirectory = Path.Combine(absoluteDirectory, nodeId);

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
        public IThreadLoggerFileWriter CreateThreadLoggerFileWriter(string theadId)
        {
            return this;
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
            _dataWriter.Dispose();
            _dataStream.Dispose();
            
            base.OnDisposing();
        }
    }
}
