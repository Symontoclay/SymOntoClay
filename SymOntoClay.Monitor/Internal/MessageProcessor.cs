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
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MessageProcessor
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly IRemoteMonitor _remoteMonitor;
        private readonly bool _hasRemoteMonitor;

        public MessageProcessor(IRemoteMonitor remoteMonitor)
        {
            _remoteMonitor = remoteMonitor;
            _hasRemoteMonitor = remoteMonitor != null;
        }

        public void ProcessMessage(BaseMessage message, IFileCache fileCache, bool enableRemoteConnection)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"enableRemoteConnection = {enableRemoteConnection}");
#endif

            var text = JsonConvert.SerializeObject(message);

#if DEBUG
            //_globalLogger.Info($"text = {text}");
#endif

            var fileName = FileCacheItemInfo.GetFileName(message.NodeId, message.ThreadId, message.MessageNumber, message.GlobalMessageNumber, message.KindOfMessage);

#if DEBUG
            //_globalLogger.Info($"fileName = {fileName}");
#endif

            fileCache.WriteFile(fileName, text);

            if (_hasRemoteMonitor && enableRemoteConnection)
            {
                var envelope = new Envelope()
                {
                    KindOfMessage = message.KindOfMessage,
                    NodeId = message.NodeId,
                    ThreadId = message.ThreadId,
                    GlobalMessageNumber = message.GlobalMessageNumber,
                    MessageNumber = message.MessageNumber,
                    Text = text
                };

#if DEBUG
                //_globalLogger.Info($"envelope = {envelope}");
#endif

                _remoteMonitor.WriteMessage(envelope);
            }
        }
    }
}
