using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal.FileCache;
using SymOntoClay.Monitor.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.Monitor.Common.Data;

namespace SymOntoClay.Monitor
{
    public class Monitor : MonitorLogger, IMonitor
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly MonitorContext _monitorContext;
        private readonly IRemoteMonitor _remoteMonitor;
        private readonly MessageProcessor _messageProcessor;
        private readonly MonitorFeatures _features;
        private readonly MonitorFileCache _fileCache;

        private readonly MessageNumberGenerator _globalMessageNumberGenerator;
        private readonly MessageNumberGenerator _messageNumberGenerator = new MessageNumberGenerator();

        /// <inheritdoc/>
        public override string Id => "monitor_core";

        public Monitor(MonitorSettings monitorSettings)
            : base(monitorSettings.OutputHandler, monitorSettings.ErrorHandler)
        {
#if DEBUG
            _globalLogger.Info($"monitorSettings = {monitorSettings}");
#endif

            _monitorContext = new MonitorContext()
            {
                OutputHandler = monitorSettings.OutputHandler,
                ErrorHandler = monitorSettings.ErrorHandler
            };

            _remoteMonitor = monitorSettings.RemoteMonitor;

            var now = DateTime.Now;
            var sessionName = $"{now.Year}_{now.Month:00}_{now.Day:00}_{now.Hour:00}_{now.Minute:00}_{now.Second:00}";

            _fileCache = new MonitorFileCache(monitorSettings.MessagesDir, sessionName);
            _monitorContext.FileCache = _fileCache;

            _messageProcessor = new MessageProcessor(_remoteMonitor);

            _monitorContext.MessageProcessor = _messageProcessor;

            _globalMessageNumberGenerator = _monitorContext.GlobalMessageNumberGenerator;

            _features = new MonitorFeatures
            {
                EnableCallMethod = true,
                EnableParameter = true,
                EnableOutput = true,
                EnableTrace = true,
                EnableDebug = true,
                EnableInfo = true,
                EnableWarn = true,
                EnableError = true,
                EnableFatal = true
            };

            _monitorContext.Features = _features;

            Init(_messageProcessor, _features, _fileCache, _globalMessageNumberGenerator, _messageNumberGenerator, string.Empty, string.Empty);
        }

        /// <inheritdoc/>
        public IMonitorNode CreateMotitorNode(string messagePointId, string nodeId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"messagePointId = {messagePointId}");
            _globalLogger.Info($"nodeId = {nodeId}");
            _globalLogger.Info($"memberName = {memberName}");
            _globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            _globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            _globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            _globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new CreateMotitorNodeMessage
                {
                    DateTimeStamp = now,
                    NodeId = nodeId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                _globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache);
            });

            return new MonitorNode(nodeId, _monitorContext);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _remoteMonitor?.Dispose();
        }
    }
}
