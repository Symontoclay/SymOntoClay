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
