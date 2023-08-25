using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorNode : IMonitorLoggerContext, IMonitorNode
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly MonitorContext _monitorContext;
        private readonly MonitorNodeContext _monitorNodeContext;
        private readonly MessageProcessor _messageProcessor;
        private readonly MonitorFeatures _features;
        private readonly MonitorNodeFileCache _fileCache;

        private readonly MessageNumberGenerator _globalMessageNumberGenerator;
        private readonly MessageNumberGenerator _messageNumberGenerator = new MessageNumberGenerator();

        private readonly string _nodeId;

        private readonly MonitorLogger _monitorLoggerImpl;

        /// <inheritdoc/>
        public string Id => _nodeId;

        public MonitorNode(string nodeId, MonitorContext monitorContext)
            : base()
        {
#if DEBUG
            _globalLogger.Info($"nodeId = {nodeId}");
#endif

            _nodeId = nodeId;

            _monitorNodeContext = new MonitorNodeContext();
            _monitorNodeContext.MonitorContext = monitorContext;
            _features = monitorContext.Features;

            _monitorNodeContext.Features = _features;

            _fileCache = monitorContext.FileCache.CreateMonitorNodeFileCache(nodeId);
            _monitorNodeContext.FileCache = _fileCache;
            _monitorNodeContext.NodeId = nodeId;

            _monitorNodeContext.OutputHandler = monitorContext.OutputHandler;
            _monitorNodeContext.ErrorHandler = monitorContext.ErrorHandler;

            _monitorContext = monitorContext;
            _globalMessageNumberGenerator = monitorContext.GlobalMessageNumberGenerator;
            _messageProcessor = monitorContext.MessageProcessor;

            _monitorLoggerImpl = new MonitorLogger(this);

            _monitorLoggerImpl.Init(_features, monitorContext.PlatformLoggers, _fileCache, _globalMessageNumberGenerator, _messageNumberGenerator, _nodeId, string.Empty);
        }

        Action<string> IMonitorLoggerContext.OutputHandler => _monitorContext.OutputHandler;
        Action<string> IMonitorLoggerContext.ErrorHandler => _monitorContext.ErrorHandler;
        MessageProcessor IMonitorLoggerContext.MessageProcessor => _messageProcessor;

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"threadId = {threadId}");
#endif

#if DEBUG
            _globalLogger.Info($"memberName = {memberName}");
            _globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            _globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            return CreateThreadLogger(messagePointId, threadId, string.Empty, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, string parentThreadId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"threadId = {threadId}");
            _globalLogger.Info($"parentThreadId = {parentThreadId}");
#endif

#if DEBUG
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

                var messageInfo = new CreateThreadLoggerMessage
                {
                    ParentThreadId = parentThreadId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = threadId,
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

            return new ThreadLogger(threadId, _monitorNodeContext);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, string methodName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.CallMethod(messagePointId, methodName, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, string parameterName, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Parameter(messagePointId, callMethodId, parameterName, parameterValue, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Output(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Trace(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Debug(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Info(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Warn(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Error(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Error(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Fatal(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Fatal(messagePointId, exception, memberName, sourceFilePath, sourceLineNumber);
        }
    }
}
