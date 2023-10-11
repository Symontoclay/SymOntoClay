using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class FileStreamsStorage: IDisposable
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public FileStreamsStorage(FileStreamsStorageOptions options)
        {
            _options = options;
        }

        private readonly FileStreamsStorageOptions _options;
        private StreamWriter _mainStreamWriter;
        private readonly Dictionary<string, Dictionary<string, StreamWriter>> _streamWritersDict = new Dictionary<string, Dictionary<string, StreamWriter>>();

        private StreamWriter MainStreamWriter => _mainStreamWriter ??= new StreamWriter(Path.Combine(_options.OutputDirectory, GetFileName(string.Empty, string.Empty)));

        public StreamWriter GetStreamWriter(string nodeId, string threadId)
        {
#if DEBUG
            //_logger.Info($"nodeId = {nodeId}");
            //_logger.Info($"threadId = {threadId}");
#endif

            if(string.IsNullOrWhiteSpace(nodeId) && string.IsNullOrWhiteSpace(threadId))
            {
                return MainStreamWriter;
            }

            if(!_options.SeparateOutputByNodes && !_options.SeparateOutputByNodes)
            {
                return MainStreamWriter;
            }

            if(string.IsNullOrWhiteSpace(nodeId))
            {
                nodeId = string.Empty;
            }

            if(string.IsNullOrWhiteSpace(threadId))
            {
                threadId = string.Empty;
            }

            if(_streamWritersDict.ContainsKey(nodeId))
            {
                var dict = _streamWritersDict[nodeId];

                if (!_options.SeparateOutputByThreads)
                {
                    return dict[string.Empty];
                }

                if (dict.ContainsKey(threadId))
                {
                    return dict[threadId];
                }

                {
                    var stream = new StreamWriter(Path.Combine(_options.OutputDirectory, GetFileName(nodeId, threadId)));
                    dict[threadId] = stream;
                    return stream;
                }
            }

            {
                var dict = new Dictionary<string, StreamWriter>();

                if(!_options.SeparateOutputByThreads)
                {
                    var stream = new StreamWriter(Path.Combine(_options.OutputDirectory, GetFileName(nodeId, string.Empty)));
                    dict[string.Empty] = stream;
                    _streamWritersDict[nodeId] = dict;
                    return stream;
                }

                {
                    var stream = new StreamWriter(Path.Combine(_options.OutputDirectory, GetFileName(nodeId, threadId)));
                    dict[threadId] = stream;
                    _streamWritersDict[nodeId] = dict;
                    return stream;
                }

            }
        }

        private string GetFileName(string nodeId, string threadId)
        {
            var sb = new StringBuilder();

            foreach (var item in _options.FileNameTemplate)
            {
                sb.Append(item.GetText(nodeId, threadId));
            }

            return sb.ToString().Replace(':', '_').Trim();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _mainStreamWriter?.Dispose();

            foreach(var nodeDict in _streamWritersDict.Values)
            {
                foreach(var threadStream in nodeDict.Values)
                {
                    threadStream.Dispose();
                }
            }

            _streamWritersDict.Clear();
        }
    }
}
