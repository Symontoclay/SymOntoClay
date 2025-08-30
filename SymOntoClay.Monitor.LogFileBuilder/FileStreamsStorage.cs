/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class FileStreamsStorage: IDisposable
    {
#if DEBUG
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public FileStreamsStorage(FileStreamsStorageOptions options)
        {
            _options = options;
            _logFileCreatorContext = options.LogFileCreatorContext;
            _mainFileName = NGetFileName(string.Empty, string.Empty);
            _fileExtension = _logFileCreatorContext.GetFileExtension();
        }

        private readonly FileStreamsStorageOptions _options;
        private readonly ILogFileCreatorContext _logFileCreatorContext;
        private readonly string _fileExtension;

        private StreamWriter _mainStreamWriter;
        private readonly string _mainFileName;

        private readonly Dictionary<string, Dictionary<string, StreamWriter>> _streamWritersDict = new Dictionary<string, Dictionary<string, StreamWriter>>();
        private readonly Dictionary<string, Dictionary<string, string>> _fileNamesDict = new Dictionary<string, Dictionary<string, string>>();

        private StreamWriter MainStreamWriter => _mainStreamWriter ??= CreateStream(Path.Combine(_options.OutputDirectory, _mainFileName));

        private StreamWriter CreateStream(string path)
        {
            if(_options.ToHtml)
            {
                path = path.Replace(_fileExtension, ".html");
            }

            var streamWriter = new StreamWriter(path);

            if(_options.ToHtml)
            {
                var content = _logFileCreatorContext.DecorateFileHeader();

                if(!string.IsNullOrWhiteSpace(content))
                {
                    streamWriter.WriteLine(content);
                }                
            }

            return streamWriter;
        }

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
                var fileNamesDict = _fileNamesDict[nodeId];

                if (!_options.SeparateOutputByThreads)
                {
                    return dict[string.Empty];
                }

                if (dict.ContainsKey(threadId))
                {
                    return dict[threadId];
                }

                {
                    var fileName = Path.Combine(_options.OutputDirectory, NGetFileName(nodeId, threadId));

#if DEBUG
                    _logger.Info($"fileName = {fileName}");
#endif

                    fileNamesDict[threadId] = fileName;
                    var stream = CreateStream(fileName);
                    stream.AutoFlush = true;
                    dict[threadId] = stream;
                    return stream;
                }
            }

            {
                var dict = new Dictionary<string, StreamWriter>();
                var fileNamesDict = new Dictionary<string, string>();

                if (!_options.SeparateOutputByThreads)
                {
                    var fileName = NGetFileName(nodeId, string.Empty);

#if DEBUG
                    _logger.Info($"fileName = {fileName}");
#endif

                    var stream = CreateStream(Path.Combine(_options.OutputDirectory, fileName));
                    stream.AutoFlush = true;
                    dict[string.Empty] = stream;
                    _streamWritersDict[nodeId] = dict;
                    fileNamesDict[string.Empty] = fileName;
                    _fileNamesDict[nodeId] = fileNamesDict;
                    return stream;
                }

                {
                    var fileName = NGetFileName(nodeId, threadId);

#if DEBUG
                    _logger.Info($"fileName = {fileName}");
#endif

                    var stream = CreateStream(Path.Combine(_options.OutputDirectory, fileName));
                    stream.AutoFlush = true;
                    dict[threadId] = stream;
                    _streamWritersDict[nodeId] = dict;
                    fileNamesDict[threadId] = fileName;
                    _fileNamesDict[nodeId] = fileNamesDict;
                    return stream;
                }
            }
        }

        private string NGetFileName(string nodeId, string threadId)
        {
#if DEBUG
            _logger.Info($"nodeId = {nodeId}");
            _logger.Info($"threadId = {threadId}");
#endif

            var sb = new StringBuilder();

            foreach (var item in _options.FileNameTemplate)
            {
                sb.Append(item.GetText(nodeId, threadId));
            }

            var result = sb.ToString().Replace(':', '_').Trim();

            if(_options.ToHtml)
            {
                var fileNameExtension = new FileInfo(result).Extension;

#if DEBUG
                _logger.Info($"fileNameExtension = {fileNameExtension}");
#endif

                result = result.Replace(fileNameExtension, ".html");
            }

            return result;
        }

        public (string AbsoluteName, string RelativeName) GetFileComplexName(string nodeId, string threadId)
        {
            var fileName = GetFileName(nodeId, threadId);

            return (Path.Combine(_options.OutputDirectory, fileName), fileName);
        }

        public string GetFileName(string nodeId, string threadId)
        {
#if DEBUG
            //_logger.Info($"nodeId = {nodeId}");
            //_logger.Info($"threadId = {threadId}");
#endif

            if (string.IsNullOrWhiteSpace(nodeId) && string.IsNullOrWhiteSpace(threadId))
            {
                return _mainFileName;
            }

            if (!_options.SeparateOutputByNodes && !_options.SeparateOutputByNodes)
            {
                return _mainFileName;
            }

            if (string.IsNullOrWhiteSpace(nodeId))
            {
                nodeId = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(threadId))
            {
                threadId = string.Empty;
            }

            if (!_options.SeparateOutputByThreads)
            {
                return _fileNamesDict[nodeId][string.Empty];
            }

#if DEBUG
            //_logger.Info($"_fileNamesDict = {JsonConvert.SerializeObject(_fileNamesDict, Formatting.Indented)}");
#endif

            return _fileNamesDict[nodeId][threadId];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
#if DEBUG
            //_logger.Info("Dispose()");
#endif

            var toHtml = _options.ToHtml;

            var content = _logFileCreatorContext.DecorateFileFooter();

            if (string.IsNullOrWhiteSpace(content))
            {
                toHtml = false;
            }

            if (_mainStreamWriter != null)
            {
                if(toHtml)
                {
                    _mainStreamWriter.WriteLine(content);
                }

                _mainStreamWriter.Dispose();
            }

            foreach(var nodeDict in _streamWritersDict.Values)
            {
                foreach(var threadStream in nodeDict.Values)
                {
                    if (toHtml)
                    {
                        threadStream.WriteLine(content);
                    }

                    threadStream.Dispose();
                }

                nodeDict.Clear();
            }

            _streamWritersDict.Clear();
        }
    }
}
