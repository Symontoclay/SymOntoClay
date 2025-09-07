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
using SymOntoClay.Common.Disposing;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Internal.FileCache;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorLogger : Disposable, IMonitorLogger
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

            _cancellationTokenSource = new CancellationTokenSource();
            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, context.CancellationToken);

            var threadingSettings = context.ThreadingSettings;

            _threadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount, 
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount, 
                context.CancellationToken);
        }

        private readonly IMonitorLoggerContext _context;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationTokenSource _linkedCancellationTokenSource;

        private readonly ICustomThreadPool _threadPool;

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

        string IMonitorLogger.Id => throw new NotImplementedException("DB6C455C-9ED3-41D1-BF0D-519E637F1CD8");

        bool IMonitorLogger.IsReal => throw new NotImplementedException("F251EF2C-DA85-4227-9EC3-63359A7EFE26");

        IMonitorFeatures IMonitorLogger.MonitorFeatures => throw new NotImplementedException("A46DAD00-A6D9-47EB-85CA-CF9D0DDAE0AF");

        /// <inheritdoc/>
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain => _context.KindOfLogicalSearchExplain;

        /// <inheritdoc/>
        public bool EnableAddingRemovingFactLoggingInStorages => _context.EnableAddingRemovingFactLoggingInStorages;

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        /// <inheritdoc/>
        public string CreateThreadId()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"methodIdentifier = {methodIdentifier.ToLabel()}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"isSync = {isSync}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var callMethodId = GetCallMethodId();

            if (!_features.EnableCallMethod)
            {
                return callMethodId;
            }

            return NCallMethod(messagePointId, methodIdentifier, string.Empty, null, isSync, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            List<MonitoredHumanizedLabel> chainOfProcessInfo,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"methodIdentifier = {methodIdentifier.ToLabel()}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"isSync = {isSync}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var callMethodId = GetCallMethodId();

            if (!_features.EnableCallMethod)
            {
                return callMethodId;
            }

            return NCallMethod(messagePointId, methodIdentifier, string.Empty, chainOfProcessInfo, isSync, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, string methodName,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"methodName = {methodName}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"isSync = {isSync}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var callMethodId = GetCallMethodId();

            if (!_features.EnableCallMethod)
            {
                return callMethodId;
            }

            return NCallMethod(messagePointId, null, methodName, null, isSync, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        [MethodForLoggingSupport]
        private string NCallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier, string altMethodName,
            List<MonitoredHumanizedLabel> chainOfProcessInfo,
            bool isSync,
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
                IsSync = isSync,
                SourceFilePath = sourceFilePath,
                SourceLineNumber = sourceLineNumber
            };

#if DEBUG
            //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

            ProcessMessage(messageInfo);

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

        private string ToBase64String(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
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

            var base64Str = ToBase64String(jsonStr);

#if DEBUG
            //_globalLogger.Info($"base64Str = {base64Str}");
#endif

            var now = DateTime.Now;

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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StartProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"processInfoId = {processInfoId}");
            //_globalLogger.Info($"processInfo = {processInfo}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableStartProcessInfo)
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

            var messageInfo = new StartProcessInfoMessage
            {
                ProcessInfo = processInfo,
                ProcessInfoId = processInfoId,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"processInfoId = {processInfoId}");
            //_globalLogger.Info($"processInfo = {processInfo}");
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

            var messageInfo = new CancelProcessInfoMessage
            {
                ProcessInfo = processInfo,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WeakCancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"processInfoId = {processInfoId}");
            //_globalLogger.Info($"processInfo = {processInfo}");
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

            var messageInfo = new WeakCancelProcessInfoMessage
            {
                ProcessInfo = processInfo,
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetProcessInfoStatus(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"processInfoId = {processInfoId}");
            //_globalLogger.Info($"processInfo = {processInfo}");
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

            var messageInfo = new SetProcessInfoStatusMessage
            {
                ProcessInfo = processInfo,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WaitProcessInfo(string messagePointId, string waitingProcessInfoId, MonitoredHumanizedLabel waitingProcessInfo, List<MonitoredHumanizedLabel> processes, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"waitingProcessInfoId = {waitingProcessInfoId}");
            //_globalLogger.Info($"waitingProcessInfo = {waitingProcessInfo}");
            //_globalLogger.Info($"processes = {processes.WriteListToString()}");
            //_globalLogger.Info($"callMethodId = {callMethodId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableWaitProcessInfo)
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

            var messageInfo = new WaitProcessInfoMessage
            {
                WaitingProcessInfo = waitingProcessInfo,
                WaitingProcessInfoId = waitingProcessInfoId,
                Processes = processes,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunLifecycleTrigger(string messagePointId, string instanceId, string holder, Enum kindOfSystemEvent,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"instanceId = {instanceId}");
            //_globalLogger.Info($"holder = {holder}");
            //_globalLogger.Info($"kindOfSystemEvent = {kindOfSystemEvent}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
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
            //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string DoTriggerSearch(string messagePointId, string instanceId, string holder, MonitoredHumanizedLabel trigger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"instanceId = {instanceId}");
            //_globalLogger.Info($"holder = {holder}");
            //_globalLogger.Info($"trigger = {trigger}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            var doTriggerSearchId = Guid.NewGuid().ToString("D");

            if (!_features.EnableDoTriggerSearch)
            {
                return doTriggerSearchId;
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

            var messageInfo = new DoTriggerSearchMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
                InstanceId = instanceId,
                Holder = holder,
                TriggerLabel = trigger,
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

            ProcessMessage(messageInfo);

            return doTriggerSearchId;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndDoTriggerSearch(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"doTriggerSearchId = {doTriggerSearchId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableEndDoTriggerSearch)
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

            var messageInfo = new EndDoTriggerSearchMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"doTriggerSearchId = {doTriggerSearchId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableSetConditionalTrigger)
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

            var messageInfo = new SetConditionalTriggerMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ResetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"doTriggerSearchId = {doTriggerSearchId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableResetConditionalTrigger)
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

            var messageInfo = new ResetConditionalTriggerMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"expr = {expr}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableRunSetExprOfConditionalTrigger)
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

            var messageInfo = new RunSetExprOfConditionalTriggerMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
                ExprLabel = expr,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndRunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"expr = {expr}");
            //_globalLogger.Info($"isSuccess = {isSuccess}");
            //_globalLogger.Info($"isPeriodic = {isPeriodic}");
            //_globalLogger.Info($"fetchedResults = {JsonConvert.SerializeObject(fetchedResults, Formatting.Indented)}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableEndRunSetExprOfConditionalTrigger)
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

            var messageInfo = new EndRunSetExprOfConditionalTriggerMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
                ExprLabel = expr,
                IsSuccess = isSuccess,
                IsPeriodic = isPeriodic,
                FetchedResults = fetchedResults,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"expr = {expr}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableRunResetExprOfConditionalTrigger)
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

            var messageInfo = new RunResetExprOfConditionalTriggerMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
                ExprLabel = expr,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndRunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"expr = {expr}");
            //_globalLogger.Info($"isSuccess = {isSuccess}");
            //_globalLogger.Info($"isPeriodic = {isPeriodic}");
            //_globalLogger.Info($"fetchedResults = {JsonConvert.SerializeObject(fetchedResults, Formatting.Indented)}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableEndRunResetExprOfConditionalTrigger)
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

            var messageInfo = new EndRunResetExprOfConditionalTriggerMessage
            {
                DoTriggerSearchId = doTriggerSearchId,
                ExprLabel = expr,
                IsSuccess = isSuccess,
                IsPeriodic = isPeriodic,
                FetchedResults = fetchedResults,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ActivateIdleAction(string messagePointId, MonitoredHumanizedLabel activatedAction,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"activatedAction = {activatedAction}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableActivateIdleAction)
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

            var messageInfo = new ActivateIdleActionMessage
            {
                ActivatedAction = activatedAction,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public ulong LogicalSearchExplain(string messagePointId, string dotStr, MonitoredHumanizedLabel query,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"dotStr = {dotStr}");
            //_globalLogger.Info($"query = {query}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableLogicalSearchExplain)
            {
                return 0u;
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

            var base64Str = ToBase64String(dotStr);

#if DEBUG
            //_globalLogger.Info($"base64Str = {base64Str}");
#endif

            var now = DateTime.Now;

            var messageInfo = new LogicalSearchExplainMessage
            {
                DotContent = base64Str,
                Query = query,
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

            ProcessMessage(messageInfo);

            return globalMessageNumber;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void AddFactOrRuleTriggerResult(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            MonitoredHumanizedLabel result,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"fact = {fact}");
            //_globalLogger.Info($"logicalStorage = {logicalStorage}");
            //_globalLogger.Info($"result = {result}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableAddFactOrRuleTriggerResult)
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

            var messageInfo = new AddFactOrRuleTriggerResultMessage
            {
                Fact = fact,
                LogicalStorage = logicalStorage,
                Result = result,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void AddFactToLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"fact = {fact}");
            //_globalLogger.Info($"logicalStorage = {logicalStorage}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableAddFactToLogicalStorage)
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

            var messageInfo = new AddFactToLogicalStorageMessage
            {
                Fact = fact,
                LogicalStorage = logicalStorage,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RemoveFactFromLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"fact = {fact}");
            //_globalLogger.Info($"logicalStorage = {logicalStorage}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableRemoveFactFromLogicalStorage)
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

            var messageInfo = new RemoveFactFromLogicalStorageMessage
            {
                Fact = fact,
                LogicalStorage = logicalStorage,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RefreshLifeTimeInLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            int newLifetime,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"fact = {fact}");
            //_globalLogger.Info($"logicalStorage = {logicalStorage}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableRefreshLifeTimeInLogicalStorage)
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

            var messageInfo = new RefreshLifeTimeInLogicalStorageMessage
            {
                Fact = fact,
                LogicalStorage = logicalStorage,
                NewLifetime = newLifetime,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void PutFactForRemovingFromLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"fact = {fact}");
            //_globalLogger.Info($"logicalStorage = {logicalStorage}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnablePutFactForRemovingFromLogicalStorage)
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

            var messageInfo = new PutFactForRemovingFromLogicalStorageMessage
            {
                Fact = fact,
                LogicalStorage = logicalStorage,
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

            ProcessMessage(messageInfo);
        }

        private static readonly object _threadTaskLockObj = new object();

        private static ulong _currentThreadTaskId;
        private static ulong _currentThreadTasksCount;

        private static (ulong TaskId, ulong TasksCount) IncreaseThreadTasksCount()
        {
            lock (_threadTaskLockObj)
            {
                return (++_currentThreadTaskId, ++_currentThreadTasksCount);
            }
        }

        private static ulong DecreaseThreadTasksCount()
        {
            lock (_threadTaskLockObj)
            {
                return --_currentThreadTasksCount;
            }
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public ulong StartThreadTask(string messagePointId,
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

            if (!_features.EnableThreadTask)
            {
                return 0u;
            }

            var tasksCountResult = IncreaseThreadTasksCount();

#if DEBUG
            //_globalLogger.Info($"tasksCountResult = {tasksCountResult}");
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

            var now = DateTime.Now;

            var messageInfo = new StartTaskMessage
            {
                TaskId = tasksCountResult.TaskId,
                TasksCount = tasksCountResult.TasksCount,
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

            ProcessMessage(messageInfo);

            return tasksCountResult.TaskId;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StopThreadTask(string messagePointId, ulong taskId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"taskId = {taskId}");
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            if (!_features.EnableThreadTask)
            {
                return;
            }

            var tasksCount = DecreaseThreadTasksCount();

#if DEBUG
            //_globalLogger.Info($"tasksCount = {tasksCount}");
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

            var now = DateTime.Now;

            var messageInfo = new StopTaskMessage
            {
                TaskId = taskId,
                TasksCount = tasksCount,
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StartBuildPlan(string messagePointId,
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

            if (!_features.EnableBuildPlan)
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

            var messageInfo = new StartBuildPlanMessage
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

            ProcessMessage(messageInfo);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StopBuildPlan(string messagePointId,
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

            if (!_features.EnableBuildPlan)
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

            var messageInfo = new StopBuildPlanMessage
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

            ProcessMessage(messageInfo);
        }

        private static readonly object _primitiveTaskLockObj = new object();

        private static ulong _currentPrimitiveTaskId;

        private static ulong GetNewPrimitiveTaskId()
        {
            lock (_primitiveTaskLockObj)
            {
                return ++_currentPrimitiveTaskId;
            } 
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

            ProcessMessage(messageInfo);
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
                    platformLogger.WriteLnRawTrace(messagePointId, message);
                }
            }

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

            ProcessMessage(messageInfo);
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
                    platformLogger.WriteLnRawDebug(messagePointId, message);
                }
            }

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

            ProcessMessage(messageInfo);
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

#if DEBUG
            //_globalLogger.Info($"_platformLoggers.Count = {_platformLoggers.Count}");
#endif

            if (_platformLoggers.Any())
            {
                foreach (var platformLogger in _platformLoggers)
                {
                    platformLogger.WriteLnRawInfo(messagePointId, message);
                }
            }

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

            ProcessMessage(messageInfo);
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
                    platformLogger.WriteLnRawWarn(messagePointId, message);
                }
            }

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

            ProcessMessage(messageInfo);
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
                    platformLogger.WriteLnRawError(messagePointId, message);
                }
            }

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

            ProcessMessage(messageInfo);
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
                    platformLogger.WriteLnRawFatal(messagePointId, message);
                }
            }

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

            ProcessMessage(messageInfo);
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

        private void ProcessMessage(BaseMessage messageInfo)
        {
            if (_context.EnableAsyncMessageCreation)
            {
                ThreadTask.Run(() => {
#if DEBUG
                    //_globalLogger.Info($"NEXT");
#endif

                    _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
                }, _threadPool, _linkedCancellationTokenSource.Token);
            }
            else
            {
                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _context.EnableRemoteConnection);
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposing()
        {
            _cancellationTokenSource.Dispose();
            _threadPool.Dispose();

            base.OnDisposing();
        }
    }
}
