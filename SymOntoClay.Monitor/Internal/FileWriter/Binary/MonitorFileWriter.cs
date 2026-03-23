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

            var indexFileName = Path.Combine(_absoluteDirectory, "Logs.idx");

#if DEBUG
            //_globalLogger.Info($"indexFileName = {indexFileName}");
#endif

            var elogFileName = Path.Combine(_absoluteDirectory, "eLogs.dat");

#if DEBUG
            _globalLogger.Info($"elogFileName = {elogFileName}");
#endif

            //_dataStream = new FileStream(dataFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.WriteThrough);
            //_indexStream = new FileStream(indexFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.WriteThrough);
            //_indexWriter = new BinaryWriter(_indexStream);

            _eStream = new FileStream(elogFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _eWriter = new BinaryWriter(_eStream);
        }

        private readonly string _absoluteDirectory;
        private readonly FileStream _dataStream;
        private readonly FileStream _indexStream;
        private readonly BinaryWriter _indexWriter;
        private readonly FileStream _eStream;
        private readonly BinaryWriter _eWriter;

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

            MonitorIndexFileRowFormat.WriteELog(_eWriter, nodeId, threadId, messageNumber, globalMessageNumber, (int)kindOfMessage, data);

            _eStream.Flush();

            /*
            var startPosition = _dataStream.Position;

#if DEBUG
            //_globalLogger.Info($"startPosition = {startPosition}");
#endif

            _dataStream.Write(data, 0, data.Length);

#if DEBUG
            //_globalLogger.Info($"_dataStream.Position (after) = {_dataStream.Position}");
#endif
            
            MonitorIndexFileRowFormat.Write(writer: _indexWriter, nodeId: nodeId, threadId: threadId, messageNumber: messageNumber, globalMessageNumber: globalMessageNumber, kindOfMessage: (int)kindOfMessage, startPosition: startPosition, dataLength: data.Length);

            _dataStream.Flush(true);//tmp
            _indexStream.Flush(true);//tmp*/
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            //_indexWriter.Dispose();
            //_dataStream.Dispose();
            //_indexStream.Dispose();

            _eWriter.Dispose();
            _eStream.Dispose();

            base.OnDisposing();
        }
    }
}
