using SymOntoClay.Common.Disposing;
using System;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary
{
    public class MonitorDataFileReader: Disposable, IDataFileReader
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public byte[] Read(string fileName, long startPosition, int dataLength)
        {
#if DEBUG
            _globalLogger.Info($"fileName = {fileName}");
            _globalLogger.Info($"startPosition = {startPosition}");
            _globalLogger.Info($"dataLength = {dataLength}");
#endif

            throw new NotImplementedException("C75562BB-F57F-4CB3-A988-A12CA587F0D3");
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            base.OnDisposing();
        }
    }
}
