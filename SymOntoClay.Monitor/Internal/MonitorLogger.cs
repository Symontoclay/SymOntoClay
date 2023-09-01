using Newtonsoft.Json;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorLogger : IMonitorLogger
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MonitorLogger(IMonitorLoggerContext context)
        {
            _context = context;

            _outputHandler = context.OutputHandler;
            _errorHandler = context.ErrorHandler;
            _messageProcessor = context.MessageProcessor;
            _features = context.Features;
            _platformLoggers = context.PlatformLoggers;
            _fileCache = context.FileCache;
            _globalMessageNumberGenerator = context.GlobalMessageNumberGenerator;
            _messageNumberGenerator = context.MessageNumberGenerator;
            _nodeId = context.NodeId;
            _threadId = context.ThreadId;
        }

        private readonly IMonitorLoggerContext _context;

        private MessageProcessor _messageProcessor;
        private IMonitorFeatures _features;
        private IFileCache _fileCache;
        private MessageNumberGenerator _globalMessageNumberGenerator;
        private IList<IPlatformLogger> _platformLoggers;

        private string _nodeId;
        private string _threadId;

        private MessageNumberGenerator _messageNumberGenerator;

        private readonly Action<string> _outputHandler;
        private readonly Action<string> _errorHandler;

        /// <inheritdoc/>
        public string Id => throw new NotImplementedException();

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        /// <inheritdoc/>
        [MethodForLoggingSupport]
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });

            return messagePointId;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
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
                    TypeName = parameterValue?.GetType().FullName ?? "null",
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
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

            _outputHandler?.Invoke(message);

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawOutput(message);
                }
            }

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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Trace(string messagePointId, string message,
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

            if (!_features.EnableTrace)
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

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawTrace(message);
                }
            }

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new TraceMessage
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Debug(string messagePointId, string message,
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

            if (!_features.EnableDebug)
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

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawDebug(message);
                }
            }

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new DebugMessage
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
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

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawInfo(message);
                }
            }

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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Warn(string messagePointId, string message,
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

            if (!_features.EnableWarn)
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

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawWarn(message);
                }
            }

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new WarnMessage
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string messagePointId, string message,
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

            if (!_features.EnableError)
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

            _errorHandler?.Invoke(message);

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawError(message);
                }
            }

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new ErrorMessage
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Error(messagePointId, exception?.ToString(), memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, string message,
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

            if (!_features.EnableFatal)
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

            _errorHandler?.Invoke(message);

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawFatal(message);
                }
            }

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new FatalMessage
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Fatal(messagePointId, exception?.ToString(), memberName, sourceFilePath, sourceLineNumber);
        }
    }
}
