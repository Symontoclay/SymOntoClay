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
using SymOntoClay.CoreHelper.DebugHelpers;
using System.Reflection;

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

        private readonly IDictionary<string, BaseMonitorSettings> _nodesSettings;
        private readonly bool _enableOnlyDirectlySetUpNodes;

        private readonly MonitorLogger _monitorLoggerImpl;

        private readonly BaseMonitorSettings _baseMonitorSettings;

        private readonly bool _TopSysEnable = true;

        /// <inheritdoc/>
        public string Id => "monitor_core";

        private readonly string _sessionName;

        public Monitor(MonitorSettings monitorSettings)
        {
#if DEBUG
            _globalLogger.Info($"monitorSettings = {monitorSettings}");
#endif
            
            _nodesSettings = monitorSettings.NodesSettings;
            _enableOnlyDirectlySetUpNodes = monitorSettings.EnableOnlyDirectlySetUpNodes;

            if(_nodesSettings == null)
            {
                _nodesSettings = new Dictionary<string, BaseMonitorSettings>();

                if(_enableOnlyDirectlySetUpNodes)
                {
                    _TopSysEnable = false;
                }
            }
            else
            {
                if(!_nodesSettings.ContainsKey(Id) && _enableOnlyDirectlySetUpNodes)
                {
                    _TopSysEnable = false;
                }
            }

            _baseMonitorSettings = monitorSettings.Clone();

            _features = _baseMonitorSettings.Features;

            if (_features == null)
            {
                _features = new MonitorFeatures
                {
                    EnableCallMethod = true,
                    EnableParameter = true,
                    EnableEndCallMethod = true,
                    EnableMethodResolving = true,
                    EnableEndMethodResolving = true,
                    EnableActionResolving = true,
                    EnableEndActionResolving = true,
                    EnableHostMethodResolving = true,
                    EnableEndHostMethodResolving = true,
                    EnableHostMethodActivation = true,
                    EnableEndHostMethodActivation = true,
                    EnableHostMethodExecution = true,
                    EnableEndHostMethodExecution = true,
                    EnableSystemExpr = true,
                    EnableOutput = true,
                    EnableTrace = true,
                    EnableDebug = true,
                    EnableInfo = true,
                    EnableWarn = true,
                    EnableError = true,
                    EnableFatal = true
                };

                _baseMonitorSettings.Features = _features;
            }

            _monitorContext = new MonitorContext()
            {
                OutputHandler = monitorSettings.OutputHandler,
                ErrorHandler = monitorSettings.ErrorHandler,
                PlatformLoggers = monitorSettings.PlatformLoggers ?? new List<IPlatformLogger>(),
                Features = _features,
                Settings = _baseMonitorSettings
            };

            _remoteMonitor = monitorSettings.RemoteMonitor;

            var now = DateTime.Now;
            _sessionName = $"{now.Year}_{now.Month:00}_{now.Day:00}_{now.Hour:00}_{now.Minute:00}_{now.Second:00}";

            _fileCache = new MonitorFileCache(monitorSettings.MessagesDir, _sessionName);
            _monitorContext.FileCache = _fileCache;

            _messageProcessor = new MessageProcessor(_remoteMonitor);

            _monitorContext.MessageProcessor = _messageProcessor;

            _globalMessageNumberGenerator = _monitorContext.GlobalMessageNumberGenerator;

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
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableCallMethod;
            }
        }

        bool IMonitorFeatures.EnableParameter
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableParameter;
            }
        }

        bool IMonitorFeatures.EnableEndCallMethod
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableEndCallMethod;
            }
        }


        bool IMonitorFeatures.EnableMethodResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableEndMethodResolving
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableEndMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableActionResolving
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableActionResolving;
            }
        }

        bool IMonitorFeatures.EnableEndActionResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableEndActionResolving;
            }
        }

        bool IMonitorFeatures.EnableHostMethodResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableHostMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableEndHostMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableHostMethodExecution
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableHostMethodExecution;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodExecution
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableEndHostMethodExecution;
            }
        }

        bool IMonitorFeatures.EnableSystemExpr
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableSystemExpr;
            }
        }

        bool IMonitorFeatures.EnableOutput
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableOutput;
            }
        }

        bool IMonitorFeatures.EnableTrace
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableTrace;
            }
        }

        bool IMonitorFeatures.EnableDebug
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableDebug;
            }
        }

        bool IMonitorFeatures.EnableInfo
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableInfo;
            }
        }

        bool IMonitorFeatures.EnableWarn
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableWarn;
            }
        }

        bool IMonitorFeatures.EnableError
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableError;
            }
        }

        bool IMonitorFeatures.EnableFatal
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Enable && _features.EnableFatal;
            }
        }

        /// <inheritdoc/>
        public bool EnableFullCallInfo => _baseMonitorSettings.EnableFullCallInfo;

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
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain => _baseMonitorSettings.KindOfLogicalSearchExplain;

        /// <inheritdoc/>
        public string LogicalSearchExplainDumpDir => _fileCache.DirectoryName;

        /// <inheritdoc/>
        public bool EnableAddingRemovingFactLoggingInStorages => _baseMonitorSettings.EnableAddingRemovingFactLoggingInStorages;

        /// <inheritdoc/>
        public bool Enable { get => _baseMonitorSettings.Enable; set => _baseMonitorSettings.Enable = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => _baseMonitorSettings.EnableRemoteConnection; set => _baseMonitorSettings.EnableRemoteConnection = value; }

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

            var classFullName = string.Empty;

            if(EnableFullCallInfo)
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

                var messageInfo = new CreateMotitorNodeMessage
                {
                    DateTimeStamp = now,
                    NodeId = nodeId,
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

                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _baseMonitorSettings.EnableRemoteConnection);
            });

            var nodeSettings = GetMotitorNodeSettings(nodeId);

            return new MonitorNode(nodeId, nodeSettings, _monitorContext);
        }

        private BaseMonitorSettings GetMotitorNodeSettings(string nodeId)
        {
            if (_nodesSettings.ContainsKey(nodeId))
            {
                return _nodesSettings[nodeId];
            }

            var nodeSettings = _baseMonitorSettings.Clone();

            if (_enableOnlyDirectlySetUpNodes)
            {
                nodeSettings.Enable = false;
            }

            return nodeSettings;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.CallMethod(messagePointId, methodIdentifier, isSynk, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, string methodName,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.CallMethod(messagePointId, methodName, isSynk, memberName, sourceFilePath, sourceLineNumber);
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
        public void Parameter(string messagePointId, string callMethodId, string parameterName, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Parameter(messagePointId, callMethodId, parameterName, parameterValue, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier methodIdentifier, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Parameter(messagePointId, callMethodId, methodIdentifier, parameterValue, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier methodIdentifier, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.Parameter(messagePointId, callMethodId, methodIdentifier, parameterValue, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndCallMethod(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndCallMethod(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
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
