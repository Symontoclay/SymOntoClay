﻿using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
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
    public class ThreadLogger : IMonitorLoggerContext, IMonitorFeatures, IThreadLogger
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly MonitorNodeContext _monitorNodeContext;
        private readonly MonitorContext _monitorContext;
        private readonly MessageProcessor _messageProcessor;
        private readonly MonitorFeatures _features;
        private readonly ThreadLoggerFileCache _fileCache;
        private readonly MessageNumberGenerator _globalMessageNumberGenerator;

        private readonly string _nodeId;
        private readonly string _threadId;

        private readonly MessageNumberGenerator _messageNumberGenerator = new MessageNumberGenerator();

        private readonly BaseMonitorSettings _baseMonitorSettings;

        private readonly MonitorLogger _monitorLoggerImpl;

        /// <inheritdoc/>
        public string Id => _threadId;

        public ThreadLogger(string threadId, MonitorNodeContext monitorNodeContext)
        {
#if DEBUG
            _globalLogger.Info($"threadId = {threadId}");
#endif

            _monitorNodeContext = monitorNodeContext;
            _baseMonitorSettings = monitorNodeContext.Settings;
            _features = monitorNodeContext.Features;
            _fileCache = monitorNodeContext.FileCache.CreateThreadLoggerFileCache(threadId);

            var monitorContext = monitorNodeContext.MonitorContext;

            _monitorContext = monitorContext;
            _messageProcessor = monitorContext.MessageProcessor;
            _globalMessageNumberGenerator = monitorContext.GlobalMessageNumberGenerator;

            _nodeId = _monitorNodeContext.NodeId;
            _threadId = threadId;

            _monitorLoggerImpl = new MonitorLogger(this);
        }

        Action<string> IMonitorLoggerContext.OutputHandler => _monitorNodeContext.OutputHandler;
        Action<string> IMonitorLoggerContext.ErrorHandler => _monitorNodeContext.ErrorHandler;
        MessageProcessor IMonitorLoggerContext.MessageProcessor => _messageProcessor;
        IMonitorFeatures IMonitorLoggerContext.Features => this;
        IList<IPlatformLogger> IMonitorLoggerContext.PlatformLoggers => _monitorNodeContext.MonitorContext.PlatformLoggers;
        IFileCache IMonitorLoggerContext.FileCache => _fileCache;
        MessageNumberGenerator IMonitorLoggerContext.GlobalMessageNumberGenerator => _globalMessageNumberGenerator;
        MessageNumberGenerator IMonitorLoggerContext.MessageNumberGenerator => _messageNumberGenerator;
        string IMonitorLoggerContext.NodeId => _nodeId;
        string IMonitorLoggerContext.ThreadId => _threadId;

        bool IMonitorLoggerContext.EnableRemoteConnection => _baseMonitorSettings.EnableRemoteConnection && _monitorContext.Settings.EnableRemoteConnection;

        bool IMonitorFeatures.EnableCallMethod
        {   
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableCallMethod;
            }
        }

        bool IMonitorFeatures.EnableParameter
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableParameter;
            }
        }

        bool IMonitorFeatures.EnableEndCallMethod
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndCallMethod;
            }
        }

        bool IMonitorFeatures.EnableMethodResolving 
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableEndMethodResolving 
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableActionResolving
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableActionResolving;
            }
        }

        bool IMonitorFeatures.EnableEndActionResolving
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndActionResolving;
            }
        }

        bool IMonitorFeatures.EnableHostMethodResolving
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableHostMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodResolving
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndHostMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableHostMethodExecution
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableHostMethodExecution;
            }
        }

        bool IMonitorFeatures.EnableHostMethodActivation
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableHostMethodActivation;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodActivation
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndHostMethodActivation;
            }
        }

        bool IMonitorFeatures.EnableHostMethodStarting
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableHostMethodStarting;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodStarting
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndHostMethodStarting;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodExecution
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndHostMethodExecution;
            }
        }

        bool IMonitorFeatures.EnableSystemExpr
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableSystemExpr;
            }
        }

        bool IMonitorFeatures.EnableOutput
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableOutput;
            }
        }

        bool IMonitorFeatures.EnableTrace
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableTrace;
            }
        }

        bool IMonitorFeatures.EnableDebug
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableDebug;
            }
        }

        bool IMonitorFeatures.EnableInfo
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableInfo;
            }
        }

        bool IMonitorFeatures.EnableWarn
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableWarn;
            }
        }

        bool IMonitorFeatures.EnableError
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableError;
            }
        }

        bool IMonitorFeatures.EnableFatal
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableFatal;
            }
        }

        /// <inheritdoc/>
        public bool EnableFullCallInfo => _baseMonitorSettings.EnableFullCallInfo && _monitorContext.Settings.EnableFullCallInfo;

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
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndCallMethod)} = {monitorFeatures.EnableEndCallMethod}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableMethodResolving)} = {monitorFeatures.EnableMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndMethodResolving)} = {monitorFeatures.EnableEndMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableActionResolving)} = {monitorFeatures.EnableActionResolving}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndActionResolving)} = {monitorFeatures.EnableEndActionResolving}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableHostMethodResolving)} = {monitorFeatures.EnableHostMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndHostMethodResolving)} = {monitorFeatures.EnableEndHostMethodResolving}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableHostMethodActivation)} = {monitorFeatures.EnableHostMethodActivation}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndHostMethodActivation)} = {monitorFeatures.EnableEndHostMethodActivation}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableHostMethodStarting)} = {monitorFeatures.EnableHostMethodStarting}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndHostMethodStarting)} = {monitorFeatures.EnableEndHostMethodStarting}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableHostMethodExecution)} = {monitorFeatures.EnableHostMethodExecution}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndHostMethodExecution)} = {monitorFeatures.EnableEndHostMethodExecution}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableSystemExpr)} = {monitorFeatures.EnableSystemExpr}");
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
        public void MethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.MethodResolving(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndMethodResolving(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.ActionResolving(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndActionResolving(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.HostMethodResolving(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndHostMethodResolving(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.HostMethodActivation(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndHostMethodActivation(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.HostMethodStarting(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndHostMethodStarting(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.HostMethodExecution(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndHostMethodExecution(messagePointId, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SystemExpr(string messagePointId, string callMethodId, string exprLabel, object exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.SystemExpr(messagePointId, callMethodId, exprLabel, exprValue, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SystemExpr(string messagePointId, string callMethodId, string exprLabel, IMonitoredObject exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.SystemExpr(messagePointId, callMethodId, exprLabel, exprValue, memberName, sourceFilePath, sourceLineNumber);
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
