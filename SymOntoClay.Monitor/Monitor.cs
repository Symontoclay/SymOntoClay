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
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System.Threading;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Threading;
using SymOntoClay.Serialization.Settings;
using SymOntoClay.Serialization;
using SymOntoClay.Serialization.SmartValues;
using System.Runtime;

namespace SymOntoClay.Monitor
{
    public class Monitor : Disposable, IMonitorLoggerContext, IMonitorFeatures, IMonitor
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        [SocSerializableExternalSettings("")]
        private readonly MonitorSettings _monitorSettings;

        private readonly MonitorContext _monitorContext;
        private readonly SmartValue<IRemoteMonitor> _remoteMonitor;
        private readonly MessageProcessor _messageProcessor;
        private readonly SmartValue<MonitorFeatures> _features;
        private readonly MonitorFileCache _fileCache;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private LinkedCancellationTokenSourceSerializationSettings _linkedCancellationTokenSourceSettings;

        [SocObjectSerializationSettings(nameof(_linkedCancellationTokenSourceSettings))]
        private readonly CancellationTokenSource _linkedCancellationTokenSource;

        private CustomThreadPoolSerializationSettings _threadPoolSerializationSettings;

        [SocObjectSerializationSettings(nameof(_threadPoolSerializationSettings))]
        private readonly ICustomThreadPool _threadPool;

        private readonly MessageNumberGenerator _globalMessageNumberGenerator;
        private readonly MessageNumberGenerator _messageNumberGenerator = new MessageNumberGenerator();

        private readonly SmartValue<IDictionary<string, BaseMonitorSettings>> _nodesSettings;
        private readonly SmartValue<bool> _enableOnlyDirectlySetUpNodes;

        private readonly MonitorLogger _monitorLoggerImpl;

        private readonly SmartValue<BaseMonitorSettings> _baseMonitorSettings;

        private readonly bool _TopSysEnable = true;

        /// <inheritdoc/>
        public string Id => "monitor_core";

        private readonly string _sessionName;

        public Monitor(MonitorSettings monitorSettings)
        {
#if DEBUG
            //_globalLogger.Info($"monitorSettings = {monitorSettings}");
#endif

            _monitorSettings = monitorSettings;
            var settingType = typeof(MonitorSettings);
            var holderKey = string.Empty;

            _cancellationTokenSource = new CancellationTokenSource();

            _linkedCancellationTokenSourceSettings = new LinkedCancellationTokenSourceSerializationSettings()
            {
                Token1 = _cancellationTokenSource.Token,
                Token2 = monitorSettings.CancellationToken
            };
            
            _linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, monitorSettings.CancellationToken);

            var threadingSettings = monitorSettings.ThreadingSettings;

            var minThreadsCount = new ExternalSettingsSmartValue<int?>(threadingSettings?.MinThreadsCount, settingType,
                new List<string> { nameof(monitorSettings.ThreadingSettings), nameof(monitorSettings.ThreadingSettings.MinThreadsCount) },
                GetType(), holderKey);

            var maxThreadsCount = new ExternalSettingsSmartValue<int?>(threadingSettings?.MaxThreadsCount, settingType,
                new List<string> { nameof(monitorSettings.ThreadingSettings), nameof(monitorSettings.ThreadingSettings.MaxThreadsCount) },
                GetType(), holderKey);

            _threadPoolSerializationSettings = new CustomThreadPoolSerializationSettings()
            {
                MinThreadsCount = minThreadsCount,
                MaxThreadsCount = maxThreadsCount,
                CancellationToken = _linkedCancellationTokenSource.Token
            };

            _threadPool = new CustomThreadPool(minThreadsCount.Value ?? DefaultCustomThreadPoolSettings.MinThreadsCount,
                maxThreadsCount.Value ?? DefaultCustomThreadPoolSettings.MaxThreadsCount,
                _linkedCancellationTokenSource.Token);

            _nodesSettings = new ExternalSettingsSmartValue<IDictionary<string, BaseMonitorSettings>>(monitorSettings.NodesSettings, settingType, nameof(monitorSettings.NodesSettings), GetType(), holderKey);
            
            _enableOnlyDirectlySetUpNodes = new ExternalSettingsSmartValue<bool>(monitorSettings.EnableOnlyDirectlySetUpNodes, settingType,
                nameof(monitorSettings.EnableOnlyDirectlySetUpNodes), GetType(), holderKey);

            if(_nodesSettings.Value == null)
            {
                _nodesSettings.SetValue(new Dictionary<string, BaseMonitorSettings>());

                if(_enableOnlyDirectlySetUpNodes.Value)
                {
                    _TopSysEnable = false;
                }
            }
            else
            {
                if(!_nodesSettings.Value.ContainsKey(Id) && _enableOnlyDirectlySetUpNodes.Value)
                {
                    _TopSysEnable = false;
                }
            }

            _baseMonitorSettings = monitorSettings.Clone();

            _features = new ExternalSettingsSmartValue<MonitorFeatures>(monitorSettings.Features, settingType, nameof(monitorSettings.Features), GetType(), holderKey);

            if (_features.Value == null)
            {
                _features.SetValue(new MonitorFeatures
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
                    EnableHostMethodStarting = true,
                    EnableEndHostMethodStarting = true,
                    EnableHostMethodExecution = true,
                    EnableEndHostMethodExecution = true,
                    EnableSystemExpr = true,
                    EnableCodeFrame = true,
                    EnableLeaveThreadExecutor = true,
                    EnableGoBackToPrevCodeFrame = true,
                    EnableStartProcessInfo = true,
                    EnableCancelProcessInfo = true,
                    EnableWeakCancelProcessInfo = true,
                    EnableCancelInstanceExecution = true,
                    EnableSetExecutionCoordinatorStatus = true,
                    EnableSetProcessInfoStatus = true,
                    EnableWaitProcessInfo = true,
                    EnableRunLifecycleTrigger = true,
                    EnableDoTriggerSearch = true,
                    EnableEndDoTriggerSearch = true,
                    EnableSetConditionalTrigger = true,
                    EnableResetConditionalTrigger = true,
                    EnableRunSetExprOfConditionalTrigger = true,
                    EnableEndRunSetExprOfConditionalTrigger = true,
                    EnableRunResetExprOfConditionalTrigger = true,
                    EnableEndRunResetExprOfConditionalTrigger = true,
                    EnableActivateIdleAction = true,

                    EnableTasks = true,

                    EnableOutput = true,
                    EnableTrace = true,
                    EnableDebug = true,
                    EnableInfo = true,
                    EnableWarn = true,
                    EnableError = true,
                    EnableFatal = true
                });
            }

            _monitorContext = new MonitorContext()
            {
                PlatformLoggers = monitorSettings.PlatformLoggers ?? new List<IPlatformLogger>(),
                Features = _features,
                Settings = _baseMonitorSettings,
                CancellationToken = _linkedCancellationTokenSource.Token,
                ThreadingSettings = monitorSettings.ThreadingSettings
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
        public string SessionDirectoryFullName => _fileCache.AbsoluteDirectoryName.Value;

        MessageProcessor IMonitorLoggerContext.MessageProcessor => _messageProcessor;
        IMonitorFeatures IMonitorLoggerContext.Features => this;
        SmartValue<IList<IPlatformLogger>> IMonitorLoggerContext.PlatformLoggers => _monitorContext.PlatformLoggers;
        IFileCache IMonitorLoggerContext.FileCache => _fileCache;
        MessageNumberGenerator IMonitorLoggerContext.GlobalMessageNumberGenerator => _globalMessageNumberGenerator;
        MessageNumberGenerator IMonitorLoggerContext.MessageNumberGenerator => _messageNumberGenerator;
        string IMonitorLoggerContext.NodeId => string.Empty;
        string IMonitorLoggerContext.ThreadId => string.Empty;

        CancellationToken IMonitorLoggerContext.CancellationToken => _linkedCancellationTokenSource.Token;
        SmartValue<CustomThreadPoolSettings> IMonitorLoggerContext.ThreadingSettings => _monitorContext.ThreadingSettings;

        /// <inheritdoc/>
        public bool IsReal => true;

        /// <inheritdoc/>
        public IMonitorFeatures MonitorFeatures => this;

        bool IMonitorFeatures.EnableCallMethod
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableCallMethod;
            }
        }

        bool IMonitorFeatures.EnableParameter
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableParameter;
            }
        }

        bool IMonitorFeatures.EnableEndCallMethod
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndCallMethod;
            }
        }


        bool IMonitorFeatures.EnableMethodResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableEndMethodResolving
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableActionResolving
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableActionResolving;
            }
        }

        bool IMonitorFeatures.EnableEndActionResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndActionResolving;
            }
        }

        bool IMonitorFeatures.EnableHostMethodResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableHostMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodResolving
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndHostMethodResolving;
            }
        }

        bool IMonitorFeatures.EnableHostMethodActivation
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableHostMethodActivation;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodActivation
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndHostMethodActivation;
            }
        }

        bool IMonitorFeatures.EnableHostMethodStarting
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableHostMethodStarting;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodStarting
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndHostMethodStarting;
            }
        }

        bool IMonitorFeatures.EnableHostMethodExecution
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableHostMethodExecution;
            }
        }

        bool IMonitorFeatures.EnableEndHostMethodExecution
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndHostMethodExecution;
            }
        }

        bool IMonitorFeatures.EnableSystemExpr
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableSystemExpr;
            }
        }

        bool IMonitorFeatures.EnableCodeFrame
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableCodeFrame;
            }
        }

        bool IMonitorFeatures.EnableLeaveThreadExecutor
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableLeaveThreadExecutor;
            }
        }

        bool IMonitorFeatures.EnableGoBackToPrevCodeFrame
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableGoBackToPrevCodeFrame;
            }
        }

        bool IMonitorFeatures.EnableStartProcessInfo
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableStartProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableCancelProcessInfo
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableCancelProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableWeakCancelProcessInfo
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableWeakCancelProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableCancelInstanceExecution 
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableCancelInstanceExecution;
            }
        }

        bool IMonitorFeatures.EnableSetExecutionCoordinatorStatus 
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableSetExecutionCoordinatorStatus;
            }
        }

        bool IMonitorFeatures.EnableSetProcessInfoStatus 
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableSetProcessInfoStatus;
            }
        }

        bool IMonitorFeatures.EnableWaitProcessInfo
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableWaitProcessInfo;
            }
        }

        bool IMonitorFeatures.EnableRunLifecycleTrigger
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableRunLifecycleTrigger;
            }
        }

        bool IMonitorFeatures.EnableDoTriggerSearch 
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableDoTriggerSearch;
            }
        }

        bool IMonitorFeatures.EnableEndDoTriggerSearch
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndDoTriggerSearch;
            }
        }

        bool IMonitorFeatures.EnableSetConditionalTrigger
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableSetConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableResetConditionalTrigger
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableResetConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableRunSetExprOfConditionalTrigger
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableRunSetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableEndRunSetExprOfConditionalTrigger
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndRunSetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableRunResetExprOfConditionalTrigger
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableRunResetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.EnableEndRunResetExprOfConditionalTrigger
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableEndRunResetExprOfConditionalTrigger;
            }
        }

        bool IMonitorFeatures.IsEnabledAnyConditionalTriggerFeature
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && (_features.Value.EnableDoTriggerSearch || _features.Value.EnableEndDoTriggerSearch || _features.Value.EnableSetConditionalTrigger ||
                    _features.Value.EnableResetConditionalTrigger || _features.Value.EnableRunSetExprOfConditionalTrigger || _features.Value.EnableEndRunSetExprOfConditionalTrigger || _features.Value.EnableRunResetExprOfConditionalTrigger ||
                    _features.Value.EnableEndRunResetExprOfConditionalTrigger);
            }
        }

        bool IMonitorFeatures.EnableActivateIdleAction
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableActivateIdleAction;
            }
        }

        bool IMonitorFeatures.EnableTasks 
        { 
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableTasks;
            }
        }

        bool IMonitorFeatures.EnableOutput
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableOutput;
            }
        }

        bool IMonitorFeatures.EnableTrace
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableTrace;
            }
        }

        bool IMonitorFeatures.EnableDebug
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableDebug;
            }
        }

        bool IMonitorFeatures.EnableInfo
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableInfo;
            }
        }

        bool IMonitorFeatures.EnableWarn
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableWarn;
            }
        }

        bool IMonitorFeatures.EnableError
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableError;
            }
        }

        bool IMonitorFeatures.EnableFatal
        {
            get
            {
                return _TopSysEnable && _baseMonitorSettings.Value.Enable && _features.Value.EnableFatal;
            }
        }

        /// <inheritdoc/>
        public bool EnableFullCallInfo => _baseMonitorSettings.Value.EnableFullCallInfo;

        /// <inheritdoc/>
        public bool EnableAsyncMessageCreation => _baseMonitorSettings.Value.EnableAsyncMessageCreation;

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
            sb.AppendLine($"{spaces}{nameof(IMonitorFeatures.EnableTasks)} = {monitorFeatures.EnableTasks}");
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
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain => _baseMonitorSettings.Value.KindOfLogicalSearchExplain;

        /// <inheritdoc/>
        public bool EnableAddingRemovingFactLoggingInStorages => _baseMonitorSettings.Value.EnableAddingRemovingFactLoggingInStorages;

        /// <inheritdoc/>
        public bool Enable { get => _baseMonitorSettings.Value.Enable; set => _baseMonitorSettings.Value.Enable = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => _baseMonitorSettings.Value.EnableRemoteConnection; set => _baseMonitorSettings.Value.EnableRemoteConnection = value; }

        /// <inheritdoc/>
        public string CreateThreadId()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public IMonitorNode CreateMotitorNode(string messagePointId, string nodeId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
#if DEBUG
            //_globalLogger.Info($"messagePointId = {messagePointId}");
            //_globalLogger.Info($"nodeId = {nodeId}");
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

            if(EnableFullCallInfo)
            {
                var callInfo = DiagnosticsHelper.GetCallInfo();

                classFullName = callInfo.ClassFullName;
                memberName = callInfo.MethodName;
            }

            var now = DateTime.Now;

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
            //_globalLogger.Info($"messageInfo = {messageInfo}");
#endif

            if(EnableAsyncMessageCreation)
            {
                ThreadTask.Run(() => {
#if DEBUG
                    //_globalLogger.Info($"NEXT");
#endif

                    _messageProcessor.ProcessMessage(messageInfo, _fileCache, _baseMonitorSettings.Value.EnableRemoteConnection);
                }, _threadPool, _linkedCancellationTokenSource.Token);
            }
            else
            {
                _messageProcessor.ProcessMessage(messageInfo, _fileCache, _baseMonitorSettings.Value.EnableRemoteConnection);
            }

            var nodeSettings = GetMotitorNodeSettings(nodeId);

            return new MonitorNode(nodeId, nodeSettings, _monitorContext);
        }

        private SmartValue<BaseMonitorSettings> GetMotitorNodeSettings(string nodeId)
        {
            if (_nodesSettings.ContainsKey(nodeId))
            {
                return _nodesSettings[nodeId];
            }

            var nodeSettings = _baseMonitorSettings.Clone();

            if (_enableOnlyDirectlySetUpNodes.Value)
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
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            List<MonitoredHumanizedLabel> chainOfProcessInfo,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.CallMethod(messagePointId, methodIdentifier, chainOfProcessInfo, isSynk, memberName, sourceFilePath, sourceLineNumber);
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
        public void ResetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.ResetConditionalTrigger(messagePointId, doTriggerSearchId, memberName, sourceFilePath, sourceLineNumber);
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
        public ulong StartTask(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return _monitorLoggerImpl.StartTask(messagePointId, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StopTask(string messagePointId, ulong taskId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _monitorLoggerImpl.StopTask(messagePointId, taskId, memberName, sourceFilePath, sourceLineNumber);
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
            _remoteMonitor.Value?.Dispose();
            _cancellationTokenSource.Dispose();
            _threadPool.Dispose();

            base.OnDisposing();
        }
    }
}
