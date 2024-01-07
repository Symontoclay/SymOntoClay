using Newtonsoft.Json;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Internal.FileCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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

        string IMonitorLogger.Id => throw new NotImplementedException();

        bool IMonitorLogger.IsReal => throw new NotImplementedException();

        IMonitorFeatures IMonitorLogger.MonitorFeatures => throw new NotImplementedException();

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"methodIdentifier = {methodIdentifier.ToLabel()}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"isSynk = {isSynk}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var callMethodId = GetCallMethodId();

            if (!_features.EnableCallMethod)
            {
                return callMethodId;
            }

            return NCallMethod(messagePointId, methodIdentifier, string.Empty, null, isSynk, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            List<MonitoredHumanizedLabel> chainOfProcessInfo,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"methodIdentifier = {methodIdentifier.ToLabel()}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"isSynk = {isSynk}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var callMethodId = GetCallMethodId();

            if (!_features.EnableCallMethod)
            {
                return callMethodId;
            }

            return NCallMethod(messagePointId, methodIdentifier, string.Empty, chainOfProcessInfo, isSynk, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, string methodName,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"methodName = {methodName}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"isSynk = {isSynk}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var callMethodId = GetCallMethodId();

            if (!_features.EnableCallMethod)
            {
                return callMethodId;
            }

            return NCallMethod(messagePointId, null, methodName, null, isSynk, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        private string NCallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier, string altMethodName,
            List<MonitoredHumanizedLabel> chainOfProcessInfo,
            bool isSynk,
            string callMethodId,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber)
        {
            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new CallMethodMessage
                {
                    AltMethodName = altMethodName,
                    MethodLabel = methodIdentifier.ToLabel(this),
                    ChainOfProcessInfo = chainOfProcessInfo,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    CallMethodId = callMethodId,
                    MemberName = memberName,
                    IsSynk = isSynk,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });

            return callMethodId;
        }

        private static string GetCallMethodId()
        {
            return Guid.NewGuid().ToString("D");
        }

        private const string NULL_LITERAL = "null";

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, string parameterName, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"parameterName = {parameterName}");
            //_globalLogger.Info($"parameterValue = {parameterValue}");
            //_globalLogger.Info($"parameterValue?.GetType().FullName = {parameterValue?.GetType().FullName}");
            //_globalLogger.Info($"parameterValue?.GetType().IsClass = {parameterValue?.GetType().IsClass}");
            //_globalLogger.Info($"parameterValue?.GetType().IsPrimitive = {parameterValue?.GetType().IsPrimitive}");
            //_globalLogger.Info($"parameterValue?.GetType().IsEnum = {parameterValue?.GetType().IsEnum}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableParameter)
            {
                return;
            }

            NLabeledValue<ParameterMessage>(messagePointId, callMethodId, null, parameterName, parameterValue, parameterValue?.ToString() ?? NULL_LITERAL, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, string parameterName, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"parameterName = {parameterName}");
            //_globalLogger.Info($"parameterValue = {parameterValue}");
            //_globalLogger.Info($"parameterValue?.GetType().FullName = {parameterValue?.GetType().FullName}");
            //_globalLogger.Info($"parameterValue?.GetType().IsClass = {parameterValue?.GetType().IsClass}");
            //_globalLogger.Info($"parameterValue?.GetType().IsPrimitive = {parameterValue?.GetType().IsPrimitive}");
            //_globalLogger.Info($"parameterValue?.GetType().IsEnum = {parameterValue?.GetType().IsEnum}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableParameter)
            {
                return;
            }

            NLabeledValue<ParameterMessage>(messagePointId, callMethodId, null, parameterName, parameterValue?.ToMonitorSerializableObject(this), parameterValue.ToHumanizedString(this), memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier methodIdentifier, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"methodIdentifier = {methodIdentifier.ToLabel()}");
            //_globalLogger.Info($"parameterValue = {parameterValue}");
            //_globalLogger.Info($"parameterValue?.GetType().FullName = {parameterValue?.GetType().FullName}");
            //_globalLogger.Info($"parameterValue?.GetType().IsClass = {parameterValue?.GetType().IsClass}");
            //_globalLogger.Info($"parameterValue?.GetType().IsPrimitive = {parameterValue?.GetType().IsPrimitive}");
            //_globalLogger.Info($"parameterValue?.GetType().IsEnum = {parameterValue?.GetType().IsEnum}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableParameter)
            {
                return;
            }

            NLabeledValue<ParameterMessage>(messagePointId, callMethodId, methodIdentifier.ToLabel(this), string.Empty, parameterValue, parameterValue?.ToString() ?? NULL_LITERAL, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier methodIdentifier, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"methodIdentifier = {methodIdentifier.ToLabel()}");
            //_globalLogger.Info($"parameterValue = {parameterValue}");
            //_globalLogger.Info($"parameterValue?.GetType().FullName = {parameterValue?.GetType().FullName}");
            //_globalLogger.Info($"parameterValue?.GetType().IsClass = {parameterValue?.GetType().IsClass}");
            //_globalLogger.Info($"parameterValue?.GetType().IsPrimitive = {parameterValue?.GetType().IsPrimitive}");
            //_globalLogger.Info($"parameterValue?.GetType().IsEnum = {parameterValue?.GetType().IsEnum}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableParameter)
            {
                return;
            }

            NLabeledValue<ParameterMessage>(messagePointId, callMethodId, methodIdentifier.ToLabel(this), string.Empty, parameterValue?.ToMonitorSerializableObject(this), parameterValue.ToHumanizedString(this), memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        private void NLabeledValue<T>(string messagePointId, string callMethodId, MonitoredHumanizedLabel label, string altLabel, object value,
            string valueHumanizedString,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber)
            where T: BaseLabeledValueMessage, new()
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"label = {label}");
            //_globalLogger.Info($"value = {value}");
            //_globalLogger.Info($"value?.GetType().FullName = {value?.GetType().FullName}");
            //_globalLogger.Info($"value?.GetType().IsClass = {value?.GetType().IsClass}");
            //_globalLogger.Info($"value?.GetType().IsPrimitive = {value?.GetType().IsPrimitive}");
            //_globalLogger.Info($"value?.GetType().IsEnum = {value?.GetType().IsEnum}");
            //_globalLogger.Info($"valueHumanizedString = {valueHumanizedString}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var jsonStr = JsonConvert.SerializeObject(value, _jsonSerializerSettings);

#if DEBUG
            //_globalLogger.Info($"jsonStr = {jsonStr}");
#endif

            var plainTextBytes = Encoding.UTF8.GetBytes(jsonStr);
            var base64Str = Convert.ToBase64String(plainTextBytes);

#if DEBUG
            //_globalLogger.Info($"base64Str = {base64Str}");
#endif

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new T
                {
                    Label = label,
                    AltLabel = altLabel,
                    TypeName = value?.GetType().FullName ?? NULL_LITERAL,
                    Base64Content = base64Str,
                    HumanizedString = valueHumanizedString,
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndCallMethod(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableEndCallMethod)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new EndCallMethodMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void MethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableMethodResolving)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new MethodResolvingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableEndMethodResolving)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new EndMethodResolvingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableActionResolving)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new ActionResolvingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableEndActionResolving)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new EndActionResolvingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableHostMethodResolving)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new HostMethodResolvingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableEndHostMethodResolving)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new EndHostMethodResolvingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableHostMethodActivation)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new HostMethodActivationMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableEndHostMethodActivation)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new EndHostMethodActivationMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableHostMethodStarting)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new HostMethodStartingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableEndHostMethodStarting)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new EndHostMethodStartingMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableHostMethodExecution)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new HostMethodExecutionMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
#endif

            if (!_features.EnableEndHostMethodExecution)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new EndHostMethodExecutionMessage
                {
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SystemExpr(string messagePointId, string callMethodId, string exprLabel, object exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"exprLabel = {exprLabel}");
            //_globalLogger.Info($"exprValue = {exprValue}");
            //_globalLogger.Info($"exprValue?.GetType().FullName = {exprValue?.GetType().FullName}");
            //_globalLogger.Info($"exprValue?.GetType().IsClass = {exprValue?.GetType().IsClass}");
            //_globalLogger.Info($"exprValue?.GetType().IsPrimitive = {exprValue?.GetType().IsPrimitive}");
            //_globalLogger.Info($"exprValue?.GetType().IsEnum = {exprValue?.GetType().IsEnum}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableSystemExpr)
            {
                return;
            }

            NLabeledValue<SystemExprMessage>(messagePointId, callMethodId, null, exprLabel, exprValue, exprValue?.ToString() ?? NULL_LITERAL, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SystemExpr(string messagePointId, string callMethodId, string exprLabel, IMonitoredObject exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"exprLabel = {exprLabel}");
            //_globalLogger.Info($"exprValue = {exprValue}");
            //_globalLogger.Info($"exprValue?.GetType().FullName = {exprValue?.GetType().FullName}");
            //_globalLogger.Info($"exprValue?.GetType().IsClass = {exprValue?.GetType().IsClass}");
            //_globalLogger.Info($"exprValue?.GetType().IsPrimitive = {exprValue?.GetType().IsPrimitive}");
            //_globalLogger.Info($"exprValue?.GetType().IsEnum = {exprValue?.GetType().IsEnum}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableSystemExpr)
            {
                return;
            }

            NLabeledValue<SystemExprMessage>(messagePointId, callMethodId, null, exprLabel, exprValue?.ToMonitorSerializableObject(this), exprValue.ToHumanizedString(this), memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CodeFrame(string messagePointId, string humanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"humanizedStr = {humanizedStr}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableCodeFrame)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new CodeFrameMessage
                {
                    HumanizedStr = humanizedStr,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void LeaveThreadExecutor(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableLeaveThreadExecutor)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new LeaveThreadExecutorMessage
                {
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void GoBackToPrevCodeFrame(string messagePointId, int targetActionExecutionStatus, string targetActionExecutionStatusStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"targetActionExecutionStatus = {targetActionExecutionStatus}");
            //_globalLogger.Info($"targetActionExecutionStatusStr = {targetActionExecutionStatusStr}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableGoBackToPrevCodeFrame)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new GoBackToPrevCodeFrameMessage
                {
                    TargetActionExecutionStatus = targetActionExecutionStatus,
                    TargetActionExecutionStatusStr = targetActionExecutionStatusStr,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CancelProcessInfo(string messagePointId, string processInfoId, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"processInfoId = {processInfoId}");
            //_globalLogger.Info($"reasonOfChangeStatus = {reasonOfChangeStatus}");
            //_globalLogger.Info($"changers = {changers.WriteListToString()}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableCancelProcessInfo)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new CancelProcessInfoMessage
                {
                    CancelledObjId = processInfoId,
                    ReasonOfChangeStatus = Convert.ToInt32(reasonOfChangeStatus),
                    ReasonOfChangeStatusStr = reasonOfChangeStatus?.ToString(),
                    Changers = changers,
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WeakCancelProcessInfo(string messagePointId, string processInfoId, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"processInfoId = {processInfoId}");
            //_globalLogger.Info($"reasonOfChangeStatus = {reasonOfChangeStatus}");
            //_globalLogger.Info($"changers = {changers.WriteListToString()}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableWeakCancelProcessInfo)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new WeakCancelProcessInfoMessage
                {
                    CancelledObjId = processInfoId,
                    ReasonOfChangeStatus = Convert.ToInt32(reasonOfChangeStatus),
                    ReasonOfChangeStatusStr = reasonOfChangeStatus?.ToString(),
                    Changers = changers,
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CancelInstanceExecution(string messagePointId, string instanceId, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"instanceId = {instanceId}");
            //_globalLogger.Info($"reasonOfChangeStatus = {reasonOfChangeStatus}");
            //_globalLogger.Info($"changers = {changers.WriteListToString()}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableCancelInstanceExecution)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new CancelInstanceExecutionMessage
                {
                    CancelledObjId = instanceId,
                    ReasonOfChangeStatus = Convert.ToInt32(reasonOfChangeStatus),
                    ReasonOfChangeStatusStr = reasonOfChangeStatus?.ToString(),
                    Changers = changers,
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetExecutionCoordinatorStatus(string messagePointId, string executionCoordinatorId, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"executionCoordinatorId = {executionCoordinatorId}");
            //_globalLogger.Info($"status = {status}");
            //_globalLogger.Info($"prevStatus = {prevStatus}");
            //_globalLogger.Info($"changers = {changers.WriteListToString()}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableSetExecutionCoordinatorStatus)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new SetExecutionCoordinatorStatusMessage
                {
                    ObjId = executionCoordinatorId,
                    Status = Convert.ToInt32(status),
                    StatusStr = status?.ToString(),
                    PrevStatus = Convert.ToInt32(prevStatus),
                    PrevStatusStr = prevStatus?.ToString(),
                    Changers = changers,
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetProcessInfoStatus(string messagePointId, string processInfoId, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"processInfoId = {processInfoId}");
            //_globalLogger.Info($"status = {status}");
            //_globalLogger.Info($"prevStatus = {prevStatus}");
            //_globalLogger.Info($"changers = {changers.WriteListToString()}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableSetProcessInfoStatus)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                //_globalLogger.Info($"NEXT");
#endif

                var messageInfo = new SetProcessInfoStatusMessage
                {
                    ObjId = processInfoId,
                    Status = Convert.ToInt32(status),
                    StatusStr = status?.ToString(),
                    PrevStatus = Convert.ToInt32(prevStatus),
                    PrevStatusStr = prevStatus?.ToString(),
                    Changers = changers,
                    CallMethodId = callMethodId,
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            });
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunLifecycleTrigger(string messagePointId, string instanceId, string holder, Enum kindOfSystemEvent,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            _globalLogger.Info($"messagePointId = {messagePointId}");
            _globalLogger.Info($"instanceId = {instanceId}");
            _globalLogger.Info($"holder = {holder}");
            _globalLogger.Info($"kindOfSystemEvent = {kindOfSystemEvent}");
            _globalLogger.Info($"memberName = {memberName}");
            _globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            _globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableRunLifecycleTrigger)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            Task.Run(() => {
#if DEBUG
                _globalLogger.Info($"NEXT");
#endif

                var messageInfo = new RunLifecycleTriggerMessage
                {
                    InstanceId = instanceId,
                    Holder = holder,
                    Status = Convert.ToInt32(kindOfSystemEvent),
                    StatusStr = kindOfSystemEvent?.ToString(),
                    DateTimeStamp = now,
                    NodeId = _nodeId,
                    ThreadId = _threadId,
                    GlobalMessageNumber = globalMessageNumber,
                    MessageNumber = messageNumber,
                    MessagePointId = messagePointId,
                    ClassFullName = classFullName,
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
        public string DoTriggerSearch(string messagePointId, string instanceId, string holder, MonitoredHumanizedLabel trigger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return string.Empty;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndDoTriggerSearch(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ResetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndRunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableOutput)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

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
                //_globalLogger.Info($"NEXT");
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
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
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
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableTrace)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

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
                //_globalLogger.Info($"NEXT");
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
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
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
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableDebug)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

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
                //_globalLogger.Info($"NEXT");
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
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
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
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableInfo)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

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
                //_globalLogger.Info($"NEXT");
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
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
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
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableWarn)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

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
                //_globalLogger.Info($"NEXT");
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
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
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
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableError)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

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
                //_globalLogger.Info($"NEXT");
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
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
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
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"message = {message}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableFatal)
            {
                return;
            }

            var messageNumber = _messageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"messageNumber = {messageNumber}");
#endif

            var globalMessageNumber = _globalMessageNumberGenerator.GetMessageNumber();

#if DEBUG
            //_globalLogger.Info($"globalMessageNumber = {globalMessageNumber}");
#endif

            var classFullName = string.Empty;

            if (_context.EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

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
                //_globalLogger.Info($"NEXT");
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
                    ClassFullName = classFullName,
                    MemberName = memberName,
                    SourceFilePath = sourceFilePath,
                    SourceLineNumber = sourceLineNumber
                };

#if DEBUG
                //_globalLogger.Info($"messageInfo = {messageInfo}");
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
