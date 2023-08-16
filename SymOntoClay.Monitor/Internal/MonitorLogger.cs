using Newtonsoft.Json;
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
    public abstract class MonitorLogger : IMonitorLogger
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        protected MonitorLogger(Action<string> outputHandler, Action<string> errorHandler)
        {
            _outputHandler = outputHandler;
            _errorHandler = errorHandler;
        }

        private MessageProcessor _messageProcessor;
        private MonitorFeatures _features;
        private IFileCache _fileCache;
        private MessageNumberGenerator _globalMessageNumberGenerator;

        private string _nodeId;
        private string _threadId;

        private MessageNumberGenerator _messageNumberGenerator;

        private readonly Action<string> _outputHandler;
        private readonly Action<string> _errorHandler;

        protected void Init(MessageProcessor messageProcessor, MonitorFeatures features, IFileCache fileCache, MessageNumberGenerator globalMessageNumberGenerator, MessageNumberGenerator messageNumberGenerator, string nodeId, string threadId)
        {
            _messageProcessor = messageProcessor;
            _features = features;
            _fileCache = fileCache;
            _globalMessageNumberGenerator = globalMessageNumberGenerator;
            _messageNumberGenerator = messageNumberGenerator;
            _nodeId = nodeId;
            _threadId = threadId;
        }

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        /// <inheritdoc/>
        public string CallMethod(string messagePointId, string methodName,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"messagePointId = {messagePointId}");
            _globalLogger.Info($"methodName = {methodName}");
            _globalLogger.Info($"memberName = {memberName}");
            _globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            _globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableCallMethod)
            {
                return messagePointId;
            }

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

                var messageInfo = new CallMethodMessage
                {
                    MethodName = methodName,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
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

            return messagePointId;
        }

        /// <inheritdoc/>
        public void Parameter(string messagePointId, string callMethodId, string parameterName, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"messagePointId = {messagePointId}");
            _globalLogger.Info($"callMethodId = {callMethodId}");
            _globalLogger.Info($"parameterName = {parameterName}");
            _globalLogger.Info($"parameterValue = {parameterValue}");
            _globalLogger.Info($"parameterValue?.GetType().FullName = {parameterValue?.GetType().FullName}");
            _globalLogger.Info($"parameterValue?.GetType().IsClass = {parameterValue?.GetType().IsClass}");
            _globalLogger.Info($"parameterValue?.GetType().IsPrimitive = {parameterValue?.GetType().IsPrimitive}");
            _globalLogger.Info($"parameterValue?.GetType().IsEnum = {parameterValue?.GetType().IsEnum}");
            _globalLogger.Info($"memberName = {memberName}");
            _globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            _globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableParameter)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            _globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            _globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var jsonStr = JsonConvert.SerializeObject(parameterValue, _jsonSerializerSettings);

#if DEBUG
            _globalLogger.Info($"jsonStr = {jsonStr}");
#endif

            var plainTextBytes = Encoding.UTF8.GetBytes(jsonStr);
            var base64Str = Convert.ToBase64String(plainTextBytes);

#if DEBUG
            _globalLogger.Info($"base64Str = {base64Str}");
#endif

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new ParameterMessage
                {
                    ParameterName = parameterName,
                    TypeName = parameterValue?.GetType().FullName,
                    Base64Content = base64Str,
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
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
        }

        /// <inheritdoc/>
        public void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"messagePointId = {messagePointId}");
            _globalLogger.Info($"message = {message}");
            _globalLogger.Info($"memberName = {memberName}");
            _globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            _globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableOutput)
            {
                return;
            }

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

                var messageInfo = new OutputMessage
                {
                    Message = message,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
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
        }

        /// <inheritdoc/>
        public void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"messagePointId = {messagePointId}");
            _globalLogger.Info($"message = {message}");
            _globalLogger.Info($"memberName = {memberName}");
            _globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            _globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableInfo)
            {
                return;
            }

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

                var messageInfo = new InfoMessage
                {
                    Message = message,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
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
        }
    }
}
