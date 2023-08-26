﻿using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Internal.FileCache;
using SymOntoClay.Monitor.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.CoreHelper.DebugHelpers;

namespace SymOntoClay.Monitor
{
    public class Monitor : IMonitorLoggerContext, IMonitorFeatures, IMonitor
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
        private readonly KindOfLogicalSearchExplain _kindOfLogicalSearchExplain;

        private readonly bool _enableAddingRemovingFactLoggingInStorages;

        private readonly MonitorLogger _monitorLoggerImpl;

        /// <inheritdoc/>
        public string Id => "monitor_core";

        private readonly string _sessionName;

        public Monitor(MonitorSettings monitorSettings)
        {
#if DEBUG
            _globalLogger.Info($"monitorSettings = {monitorSettings}");
#endif
            
            if(!monitorSettings.Enable)
            {
                throw new NotImplementedException();
            }

            if(monitorSettings.Features != null)
            {
                throw new NotImplementedException();
            }

            if(monitorSettings.NodesSettings != null)
            {
                throw new NotImplementedException();
            }

            if(monitorSettings.EnableOnlyDirectlySetUpNodes)
            {
                throw new NotImplementedException();
            }

            if(monitorSettings.PlatformLoggers == null)
            {
                monitorSettings.PlatformLoggers = new List<IPlatformLogger>();
            }

            _kindOfLogicalSearchExplain = monitorSettings.KindOfLogicalSearchExplain;
            _enableAddingRemovingFactLoggingInStorages = monitorSettings.EnableAddingRemovingFactLoggingInStorages;

            _monitorContext = new MonitorContext()
            {
                Enable = monitorSettings.Enable,
                OutputHandler = monitorSettings.OutputHandler,
                ErrorHandler = monitorSettings.ErrorHandler,
                PlatformLoggers = monitorSettings.PlatformLoggers
            };

            _remoteMonitor = monitorSettings.RemoteMonitor;

            var now = DateTime.Now;
            _sessionName = $"{now.Year}_{now.Month:00}_{now.Day:00}_{now.Hour:00}_{now.Minute:00}_{now.Second:00}";

            _fileCache = new MonitorFileCache(monitorSettings.MessagesDir, _sessionName);
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

            _monitorLoggerImpl = new MonitorLogger(this);
        }

        /// <inheritdoc/>
        public string SessionDirectoryName => _sessionName;

        /// <inheritdoc/>
        public string SessionDirectoryFullName => _fileCache.DirectoryName;

        Action<string> IMonitorLoggerContext.OutputHandler => _monitorContext.OutputHandler;
        Action<string> IMonitorLoggerContext.ErrorHandler => _monitorContext.ErrorHandler;
        MessageProcessor IMonitorLoggerContext.MessageProcessor => _messageProcessor;
        IMonitorFeatures IMonitorLoggerContext.Features => this;
        IList<IPlatformLogger> IMonitorLoggerContext.PlatformLoggers => _monitorContext.PlatformLoggers;
        IFileCache IMonitorLoggerContext.FileCache => _fileCache;
        MessageNumberGenerator IMonitorLoggerContext.GlobalMessageNumberGenerator => _globalMessageNumberGenerator;
        MessageNumberGenerator IMonitorLoggerContext.MessageNumberGenerator => _messageNumberGenerator;
        string IMonitorLoggerContext.NodeId => string.Empty;
        string IMonitorLoggerContext.ThreadId => string.Empty;

        bool IMonitorFeatures.EnableCallMethod
        {
            get
            {
                return _features.EnableCallMethod;
            }
        }

        bool IMonitorFeatures.EnableParameter
        {
            get
            {
                return _features.EnableParameter;
            }
        }

        bool IMonitorFeatures.EnableOutput
        {
            get
            {
                return _features.EnableOutput;
            }
        }

        bool IMonitorFeatures.EnableTrace
        {
            get
            {
                return _features.EnableTrace;
            }
        }

        bool IMonitorFeatures.EnableDebug
        {
            get
            {
                return _features.EnableDebug;
            }
        }

        bool IMonitorFeatures.EnableInfo
        {
            get
            {
                return _features.EnableInfo;
            }
        }

        bool IMonitorFeatures.EnableWarn
        {
            get
            {
                return _features.EnableWarn;
            }
        }

        bool IMonitorFeatures.EnableError
        {
            get
            {
                return _features.EnableError;
            }
        }

        bool IMonitorFeatures.EnableFatal
        {
            get
            {
                return _features.EnableFatal;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            var monitorFeatures = (IMonitorFeatures)this;

            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableCallMethod)} = {monitorFeatures.EnableCallMethod}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableParameter)} = {monitorFeatures.EnableParameter}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableOutput)} = {monitorFeatures.EnableOutput}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableTrace)} = {monitorFeatures.EnableTrace}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableDebug)} = {monitorFeatures.EnableDebug}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableInfo)} = {monitorFeatures.EnableInfo}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableWarn)} = {monitorFeatures.EnableWarn}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableError)} = {monitorFeatures.EnableError}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableFatal)} = {monitorFeatures.EnableFatal}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain => _kindOfLogicalSearchExplain;

        /// <inheritdoc/>
        public string LogicalSearchExplainDumpDir => _fileCache.DirectoryName;

        /// <inheritdoc/>
        public bool EnableAddingRemovingFactLoggingInStorages => _enableAddingRemovingFactLoggingInStorages;

        /// <inheritdoc/>
        public bool Enable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }//{ get => _monitorContext.Enable; set => _monitorContext.Enable = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }//{ get => _monitorContext.EnableRemoteConnection; set => _monitorContext.EnableRemoteConnection = value; }

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

        /// <inheritdoc/>
        public void Dispose()
        {
            _remoteMonitor?.Dispose();
        }
    }
}
