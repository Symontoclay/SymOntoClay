using SymOntoClay.Common.Disposing;
using SymOntoClay.Monitor.Common.Data;
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
            _globalLogger.Info($"messagesDir = {messagesDir}");
            _globalLogger.Info($"sessionName = {sessionName}");
#endif

            _absoluteDirectory = Path.Combine(messagesDir, sessionName);

            if(!Directory.Exists(_absoluteDirectory))
            {
                Directory.CreateDirectory(_absoluteDirectory);
            }

            var dataFileName = Path.Combine(_absoluteDirectory, "Logs.dat");

#if DEBUG
            _globalLogger.Info($"dataFileName = {dataFileName}");
#endif

            var indexFileName = Path.Combine(_absoluteDirectory, "Logs.idx");

#if DEBUG
            _globalLogger.Info($"indexFileName = {indexFileName}");
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
        public string AbsoluteDirectoryName => _absoluteDirectory;

        /// <inheritdoc/>
        public IMonitorNodeFileWriter CreateMonitorNodeFileWriter(string nodeId)
        {
#if DEBUG
            _globalLogger.Info($"nodeId = {nodeId}");
#endif

            throw new NotImplementedException("8B394F50-646D-439E-8211-4E9048E7E439");
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

            var startPosition = _dataStream.Position;

#if DEBUG
            _globalLogger.Info($"startPosition = {startPosition}");
#endif

            _dataStream.Write(data, 0, data.Length);

#if DEBUG
            _globalLogger.Info($"_dataStream.Position (after) = {_dataStream.Position}");
#endif

            _indexWriter.Write(nodeId?.Length ?? 0);

            if(nodeId != null)
            {
                _indexWriter.Write(nodeId);
            }          
            
            _indexWriter.Write(threadId?.Length ?? 0);

            if(threadId != null)
            {
                _indexWriter.Write(threadId);
            }
            
            _indexWriter.Write(messageNumber);
            _indexWriter.Write(globalMessageNumber);
            _indexWriter.Write((int)kindOfMessage);
            _indexWriter.Write(startPosition);
            _indexWriter.Write(data.Length);

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
