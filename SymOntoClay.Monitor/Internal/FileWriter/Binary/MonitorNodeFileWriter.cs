using SymOntoClay.Common.Disposing;
using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.Monitor.Internal.FileWriter.Binary
{
    public class MonitorNodeFileWriter : Disposable, IMonitorNodeFileWriter
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MonitorNodeFileWriter(string nodeId, string absoluteDirectory)
        {
#if DEBUG
            _globalLogger.Info($"nodeId = {nodeId}");
            _globalLogger.Info($"absoluteDirectory = {absoluteDirectory}");
#endif

            _absoluteDirectory = Path.Combine(absoluteDirectory, nodeId);

#if DEBUG
            _globalLogger.Info($"_absoluteDirectory = {_absoluteDirectory}");
#endif

            if (!Directory.Exists(_absoluteDirectory))
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
        public IThreadLoggerFileWriter CreateThreadLoggerFileWriter(string theadId)
        {
            throw new NotImplementedException("1D1ADB3D-BDE5-427C-B412-05E3CFF56EFC");
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

            throw new NotImplementedException("DE2DB7FF-4CE2-4F8B-839D-5DDB79908475");
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
