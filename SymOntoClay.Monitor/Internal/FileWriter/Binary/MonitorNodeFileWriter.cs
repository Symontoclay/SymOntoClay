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
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
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

            var indexFileName = Path.Combine(_absoluteDirectory, "Logs.idx");

#if DEBUG
            //_globalLogger.Info($"indexFileName = {indexFileName}");
#endif

            _dataStream = new FileStream(dataFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _indexStream = new FileStream(indexFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _indexWriter = new BinaryWriter(_indexStream);
        }

        private readonly string _absoluteDirectory;
        private readonly FileStream _dataStream;
        private readonly FileStream _indexStream;
        private readonly BinaryWriter _indexWriter;

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

            var startPosition = _dataStream.Position;

#if DEBUG
            //_globalLogger.Info($"startPosition = {startPosition}");
#endif

            _dataStream.Write(data, 0, data.Length);

#if DEBUG
            //_globalLogger.Info($"_dataStream.Position (after) = {_dataStream.Position}");
#endif

            MonitorIndexFileRowFormat.Write(writer: _indexWriter, nodeId: nodeId, threadId: threadId, messageNumber: messageNumber, globalMessageNumber: globalMessageNumber, kindOfMessage: (int)kindOfMessage, startPosition: startPosition, dataLength: data.Length);

            _dataStream.Flush();//tmp
            _indexStream.Flush();//tmp
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            _indexWriter.Dispose();
            _dataStream.Dispose();
            _indexStream.Dispose();

            base.OnDisposing();
        }
    }
}
