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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Common.Disposing;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Internal.FileCache;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SymOntoClay.Monitor.Internal
{
    public class MonitorNode : Disposable, IMonitorLoggerContext, IMonitorFeatures, IMonitorNode
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
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

        private readonly BaseMonitorSettings _baseMonitorSettings;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationTokenSource _linkedCancellationTokenSource;

        private readonly ICustomThreadPool _threadPool;

        /// <inheritdoc/>
        public string Id => _nodeId;

        public MonitorNode(string nodeId, BaseMonitorSettings nodeSettings, MonitorContext monitorContext)
        {
#if DEBUG
            //_globalLogger.Info($"nodeId = {nodeId}");
#endif

            _nodeId = nodeId;

            _cancellationTokenSource = new CancellationTokenSource();
            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, monitorContext.CancellationToken);

            var threadingSettings = monitorContext.ThreadingSettings;

            _threadPool = new CustomThreadPool(threadingSettings?.MinThreadsCount ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                threadingSettings?.MaxThreadsCount ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                monitorContext.CancellationToken);

            _monitorNodeContext = new MonitorNodeContext();
            _monitorNodeContext.MonitorContext = monitorContext;

            _monitorNodeContext.CancellationToken = _linkedCancellationTokenSource.Token;
            _monitorNodeContext.ThreadingSettings = monitorContext.ThreadingSettings;

            _baseMonitorSettings = nodeSettings;

            _monitorNodeContext.Settings = _baseMonitorSettings;

            _features = _baseMonitorSettings.Features;

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
        }

        Action<string> IMonitorLoggerContext.OutputHandler => _monitorContext.OutputHandler;
        Action<string> IMonitorLoggerContext.ErrorHandler => _monitorContext.ErrorHandler;
        MessageProcessor IMonitorLoggerContext.MessageProcessor => _messageProcessor;
        IMonitorFeatures IMonitorLoggerContext.Features => this;
        IList<IPlatformLogger> IMonitorLoggerContext.PlatformLoggers => _monitorContext.PlatformLoggers;
        IFileCache IMonitorLoggerContext.FileCache => _fileCache;
        MessageNumberGenerator IMonitorLoggerContext.GlobalMessageNumberGenerator => _globalMessageNumberGenerator;
        MessageNumberGenerator IMonitorLoggerContext.MessageNumberGenerator => _messageNumberGenerator;
        string IMonitorLoggerContext.NodeId => _nodeId;
        string IMonitorLoggerContext.ThreadId => string.Empty;

        CancellationToken IMonitorLoggerContext.CancellationToken => _linkedCancellationTokenSource.Token;
        CustomThreadPoolSettings IMonitorLoggerContext.ThreadingSettings => _monitorNodeContext.ThreadingSettings;

        /// <inheritdoc/>
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain => _baseMonitorSettings.KindOfLogicalSearchExplain;

        /// <inheritdoc/>
        public bool EnableAddingRemovingFactLoggingInStorages => _baseMonitorSettings.EnableAddingRemovingFactLoggingInStorages;

        /// <inheritdoc/>
        public bool IsReal => true;

        /// <inheritdoc/>
        public IMonitorFeatures MonitorFeatures => this;

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

        bool IMonitorFeatures.EnableHostMethodExecution 
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableHostMethodExecution;
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

        bool IMonitorFeatures.EnableCodeFrame
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableCodeFrame;
            }
        }

        bool IMonitorFeatures.EnableLeaveThreadExecutor
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableLeaveThreadExecutor;
            }
        }

        bool IMonitorFeatures.EnableGoBackToPrevCodeFrame
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableGoBackToPrevCodeFrame;
            }
        }

        bool IMonitorFeatures.EnableStartProcessInfo
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableStartProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableCancelProcessInfo
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableCancelProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableWeakCancelProcessInfo
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableWeakCancelProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableCancelInstanceExecution
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableCancelInstanceExecution;
            }
        }

        bool IMonitorFeatures.EnableSetExecutionCoordinatorStatus
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableSetExecutionCoordinatorStatus;
            }
        }

        bool IMonitorFeatures.EnableSetProcessInfoStatus
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableSetProcessInfoStatus;
            }
        }

        bool IMonitorFeatures.EnableWaitProcessInfo
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableWaitProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableRunLifecycleTrigger
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableRunLifecycleTrigger;
            }
        }

        bool IMonitorFeatures.EnableDoTriggerSearch
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableDoTriggerSearch;
            }
        }

        bool IMonitorFeatures.EnableEndDoTriggerSearch
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndDoTriggerSearch;
            }
        }

        bool IMonitorFeatures.EnableSetConditionalTrigger
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableSetConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableResetConditionalTrigger
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableResetConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableRunSetExprOfConditionalTrigger
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableRunSetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableEndRunSetExprOfConditionalTrigger
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndRunSetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableRunResetExprOfConditionalTrigger
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableRunResetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableEndRunResetExprOfConditionalTrigger
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableEndRunResetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.IsEnabledAnyConditionalTriggerFeature
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && (_features.EnableDoTriggerSearch || _features.EnableEndDoTriggerSearch || _features.EnableSetConditionalTrigger ||
                    _features.EnableResetConditionalTrigger || _features.EnableRunSetExprOfConditionalTrigger || _features.EnableEndRunSetExprOfConditionalTrigger || _features.EnableRunResetExprOfConditionalTrigger ||
                    _features.EnableEndRunResetExprOfConditionalTrigger);
            }
        }

        bool IMonitorFeatures.EnableActivateIdleAction
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableActivateIdleAction;
            }
        }

        bool IMonitorFeatures.EnableLogicalSearchExplain
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableLogicalSearchExplain;
            }
        }

        bool IMonitorFeatures.EnableAddFactOrRuleTriggerResult 
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableAddFactOrRuleTriggerResult;
            }
        }

        bool IMonitorFeatures.EnableAddFactToLogicalStorage 
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableAddFactToLogicalStorage;
            }
        }

        bool IMonitorFeatures.EnableRemoveFactFromLogicalStorage 
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableRemoveFactFromLogicalStorage;
            }
        }

        bool IMonitorFeatures.EnableRefreshLifeTimeInLogicalStorage 
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableRefreshLifeTimeInLogicalStorage;
            }
        }

        bool IMonitorFeatures.EnablePutFactForRemovingFromLogicalStorage 
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnablePutFactForRemovingFromLogicalStorage;
            }
        }

        bool IMonitorFeatures.EnableThreadTask 
        { 
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableThreadTask;
            }
        }

        bool IMonitorFeatures.EnableHtn
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableHtn;
            }
        }

        bool IMonitorFeatures.EnableBuildPlan
        {
            get
            {
                return _baseMonitorSettings.Enable && _monitorContext.Settings.Enable && _features.EnableHtn && _features.EnableBuildPlan;
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
        public bool Enable { get => _baseMonitorSettings.Enable; set => _baseMonitorSettings.Enable = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => _baseMonitorSettings.EnableRemoteConnection; set => _baseMonitorSettings.EnableRemoteConnection = value; }

        /// <inheritdoc/>
        public bool EnableFullCallInfo => _baseMonitorSettings.EnableFullCallInfo && _monitorContext.Settings.EnableFullCallInfo;

        /// <inheritdoc/>
        public bool EnableAsyncMessageCreation => _baseMonitorSettings.EnableAsyncMessageCreation && _monitorContext.Settings.EnableAsyncMessageCreation;

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
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableCodeFrame)} = {monitorFeatures.EnableCodeFrame}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableLeaveThreadExecutor)} = {monitorFeatures.EnableLeaveThreadExecutor}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableGoBackToPrevCodeFrame)} = {monitorFeatures.EnableGoBackToPrevCodeFrame}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableStartProcessInfo)} = {monitorFeatures.EnableStartProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableCancelProcessInfo)} = {monitorFeatures.EnableCancelProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableWeakCancelProcessInfo)} = {monitorFeatures.EnableWeakCancelProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableCancelInstanceExecution)} = {monitorFeatures.EnableCancelInstanceExecution}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableSetExecutionCoordinatorStatus)} = {monitorFeatures.EnableSetExecutionCoordinatorStatus}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableSetProcessInfoStatus)} = {monitorFeatures.EnableSetProcessInfoStatus}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableWaitProcessInfo)} = {monitorFeatures.EnableWaitProcessInfo}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableRunLifecycleTrigger)} = {monitorFeatures.EnableRunLifecycleTrigger}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableDoTriggerSearch)} = {monitorFeatures.EnableDoTriggerSearch}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndDoTriggerSearch)} = {monitorFeatures.EnableEndDoTriggerSearch}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableSetConditionalTrigger)} = {monitorFeatures.EnableSetConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableResetConditionalTrigger)} = {monitorFeatures.EnableResetConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableRunSetExprOfConditionalTrigger)} = {monitorFeatures.EnableRunSetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndRunSetExprOfConditionalTrigger)} = {monitorFeatures.EnableEndRunSetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableRunResetExprOfConditionalTrigger)} = {monitorFeatures.EnableRunResetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableEndRunResetExprOfConditionalTrigger)} = {monitorFeatures.EnableEndRunResetExprOfConditionalTrigger}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.IsEnabledAnyConditionalTriggerFeature)} = {monitorFeatures.IsEnabledAnyConditionalTriggerFeature}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableActivateIdleAction)} = {monitorFeatures.EnableActivateIdleAction}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableLogicalSearchExplain)} = {monitorFeatures.EnableLogicalSearchExplain}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableAddFactOrRuleTriggerResult)} = {monitorFeatures.EnableAddFactOrRuleTriggerResult}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableAddFactToLogicalStorage)} = {monitorFeatures.EnableAddFactToLogicalStorage}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableRemoveFactFromLogicalStorage)} = {monitorFeatures.EnableRemoveFactFromLogicalStorage}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableRefreshLifeTimeInLogicalStorage)} = {monitorFeatures.EnableRefreshLifeTimeInLogicalStorage}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnablePutFactForRemovingFromLogicalStorage)} = {monitorFeatures.EnablePutFactForRemovingFromLogicalStorage}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableThreadTask)} = {monitorFeatures.EnableThreadTask}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableHtn)} = {monitorFeatures.EnableHtn}");
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableBuildPlan)} = {monitorFeatures.EnableBuildPlan}");
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
        public string CreateThreadId()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"threadId = {threadId}");
#endif

#if DEBUG
            //_globalLogger.Info($"memberName = {memberName}");
            //_globalLogger.Info($"sourceFilePath = {sourceFilePath}");
            //_globalLogger.Info($"sourceLineNumber = {sourceLineNumber}");
#endif

            return CreateThreadLogger(messagePointId, threadId, string.Empty, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public IThreadLogger CreateThreadLogger(string messagePointId, string threadId, string parentThreadId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"threadId = {threadId}");
            //_globalLogger.Info($"parentThreadId = {parentThreadId}");
#endif

#if DEBUG
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

            if (EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

            var messageInfo = new CreateThreadLoggerMessage
            {
                ParentThreadId = parentThreadId,
                DateTimeStamp = now,
                NodeId = _nodeId,
                ThreadId = threadId,
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

            if(EnableAsyncMessageCreation)
            {
                ThreadTask.Run(() => {
#if DEBUG
                    //_globalLogger.Info($"NEXT");
#endif

                    _messageProcessor.ProcessMessage(messageInfo, _fileCache, _baseMonitorSettings.EnableRemoteConnection && _monitorContext.Settings.EnableRemoteConnection);
                }, _threadPool, _linkedCancellationTokenSource.Token);
            }
            else
            {
                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _baseMonitorSettings.EnableRemoteConnection && _monitorContext.Settings.EnableRemoteConnection);
            }

            return new ThreadLogger(threadId, _monitorNodeContext);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.CallMethod(messagePointId, methodIdentifier, isSync, memberName, sourceFilePath, sourceLineNumber);
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
            return _monitorLoggerImpl.CallMethod(messagePointId, methodIdentifier, chainOfProcessInfo, isSync, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, string methodName,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.CallMethod(messagePointId, methodName, isSync, memberName, sourceFilePath, sourceLineNumber);
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
        public void CodeFrame(string messagePointId, string humanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.CodeFrame(messagePointId, humanizedStr, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void LeaveThreadExecutor(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.LeaveThreadExecutor(messagePointId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void GoBackToPrevCodeFrame(string messagePointId, int targetActionExecutionStatus, string targetActionExecutionStatusStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.GoBackToPrevCodeFrame(messagePointId, targetActionExecutionStatus, targetActionExecutionStatusStr, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StartProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.StartProcessInfo(messagePointId, processInfoId, processInfo, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.CancelProcessInfo(messagePointId, processInfoId, processInfo, reasonOfChangeStatus, changers, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WeakCancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.WeakCancelProcessInfo(messagePointId, processInfoId, processInfo, reasonOfChangeStatus, changers, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CancelInstanceExecution(string messagePointId, string instanceId, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.CancelInstanceExecution(messagePointId, instanceId, reasonOfChangeStatus, changers, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetExecutionCoordinatorStatus(string messagePointId, string executionCoordinatorId, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.SetExecutionCoordinatorStatus(messagePointId, executionCoordinatorId, status, prevStatus, changers, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetProcessInfoStatus(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.SetProcessInfoStatus(messagePointId, processInfoId, processInfo, status, prevStatus, changers, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WaitProcessInfo(string messagePointId, string waitingProcessInfoId, MonitoredHumanizedLabel waitingProcessInfo, List<MonitoredHumanizedLabel> processes, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.WaitProcessInfo(messagePointId, waitingProcessInfoId, waitingProcessInfo, processes, callMethodId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunLifecycleTrigger(string messagePointId, string instanceId, string holder, Enum kindOfSystemEvent,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.RunLifecycleTrigger(messagePointId, instanceId, holder, kindOfSystemEvent, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string DoTriggerSearch(string messagePointId, string instanceId, string holder, MonitoredHumanizedLabel trigger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.DoTriggerSearch(messagePointId, instanceId, holder, trigger, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndDoTriggerSearch(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndDoTriggerSearch(messagePointId, doTriggerSearchId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.SetConditionalTrigger(messagePointId, doTriggerSearchId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.RunSetExprOfConditionalTrigger(messagePointId, doTriggerSearchId, expr, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ResetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.ResetConditionalTrigger(messagePointId, doTriggerSearchId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndRunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndRunSetExprOfConditionalTrigger(messagePointId, doTriggerSearchId, expr, isSuccess, isPeriodic, fetchedResults, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.RunResetExprOfConditionalTrigger(messagePointId, doTriggerSearchId, expr, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndRunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.EndRunResetExprOfConditionalTrigger(messagePointId, doTriggerSearchId, expr, isSuccess, isPeriodic, fetchedResults, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ActivateIdleAction(string messagePointId, MonitoredHumanizedLabel activatedAction,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.ActivateIdleAction(messagePointId, activatedAction, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public ulong LogicalSearchExplain(string messagePointId, string dotStr, MonitoredHumanizedLabel query,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.LogicalSearchExplain(messagePointId, dotStr, query, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void AddFactOrRuleTriggerResult(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            MonitoredHumanizedLabel result,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.AddFactOrRuleTriggerResult(messagePointId, fact, logicalStorage, result, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void AddFactToLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.AddFactToLogicalStorage(messagePointId, fact, logicalStorage, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RemoveFactFromLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.RemoveFactFromLogicalStorage(messagePointId, fact, logicalStorage, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RefreshLifeTimeInLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            int newLifetime,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.RefreshLifeTimeInLogicalStorage(messagePointId, fact, logicalStorage, newLifetime, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void PutFactForRemovingFromLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.PutFactForRemovingFromLogicalStorage(messagePointId, fact, logicalStorage, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public ulong StartThreadTask(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.StartThreadTask(messagePointId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StopThreadTask(string messagePointId, ulong taskId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.StopThreadTask(messagePointId, taskId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StartBuildPlan(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.StartBuildPlan(messagePointId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StopBuildPlan(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.StopBuildPlan(messagePointId, memberName, sourceFilePath, sourceLineNumber);
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
        protected override void OnDisposing()
        {
            _cancellationTokenSource.Dispose();
            _threadPool.Dispose();

            base.OnDisposing();
        }
    }
}
