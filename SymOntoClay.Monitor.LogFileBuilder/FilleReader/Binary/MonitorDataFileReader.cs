using SymOntoClay.Common.Disposing;
using System;
using System.Collections.Generic;
using System.IO;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary
{
    public class MonitorDataFileReader: Disposable, IDataFileReader
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly Dictionary<string, FileStream> _fileStreamsDict = new Dictionary<string, FileStream>();

        /// <inheritdoc/>
        public byte[] Read(string fileName, long startPosition, int dataLength)
        {
#if DEBUG
            _globalLogger.Info($"fileName = {fileName}");
            _globalLogger.Info($"startPosition = {startPosition}");
            _globalLogger.Info($"dataLength = {dataLength}");
#endif

            var fs = GetFileStream(fileName);
            fs.Position = startPosition;
            var data = new byte[dataLength];
            fs.Read(data, 0, data.Length);
            
            return data;
        }

        private FileStream GetFileStream(string fileName)
        {
            if(_fileStreamsDict.TryGetValue(fileName, out var fsValue))
            {
                return fsValue;
            }
            
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            _fileStreamsDict[fileName] = fs;
            return fs;
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            foreach(var item in _fileStreamsDict)
            {
                item.Value.Dispose();
            }

            base.OnDisposing();
        }
    }
}
