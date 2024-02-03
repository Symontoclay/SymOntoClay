using NLog;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.NLog
{
    public class MonitorLoggerNLogImpementation: IMonitorLogger
    {
        /// <summary>
        /// Gets default instance of the class.
        /// </summary>
        public static MonitorLoggerNLogImpementation Instance = new MonitorLoggerNLogImpementation();

        public MonitorLoggerNLogImpementation()
            : this(LogManager.GetCurrentClassLogger())
        {
        }

        public MonitorLoggerNLogImpementation(Logger logger)
        {
            _logger = logger;
        }

        private readonly Logger _logger;

        /// <inheritdoc/>
        public string Id => "MonitorLoggerNLogImpementation";

        /// <inheritdoc/>
        public bool IsReal => false;

        /// <inheritdoc/>
        public KindOfLogicalSearchExplain KindOfLogicalSearchExplain => KindOfLogicalSearchExplain.None;

        /// <inheritdoc/>
        public string LogicalSearchExplainDumpDir => string.Empty;

        /// <inheritdoc/>
        public bool EnableAddingRemovingFactLoggingInStorages => false;

        IMonitorFeatures IMonitorLogger.MonitorFeatures => throw new NotImplementedException();

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, IMonitoredMethodIdentifier methodIdentifier,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string CallMethod(string messagePointId, string methodName,
            bool isSynk,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, string parameterName, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, string parameterName, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier methodIdentifier, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier methodIdentifier, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndCallMethod(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void MethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void HostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndHostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SystemExpr(string messagePointId, string callMethodId, string exprLabel, object exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SystemExpr(string messagePointId, string callMethodId, string exprLabel, IMonitoredObject exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CodeFrame(string messagePointId, string humanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void LeaveThreadExecutor(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void GoBackToPrevCodeFrame(string messagePointId, int targetActionExecutionStatus, string targetActionExecutionStatusStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void StartProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WeakCancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void CancelInstanceExecution(string messagePointId, string processInfoId, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetExecutionCoordinatorStatus(string messagePointId, string executionCoordinatorId, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetProcessInfoStatus(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void WaitProcessInfo(string messagePointId, string waitingProcessInfoId, MonitoredHumanizedLabel waitingProcessInfo, List<MonitoredHumanizedLabel> processes, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunLifecycleTrigger(string messagePointId, string instanceId, string holder, Enum kindOfSystemEvent,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public string DoTriggerSearch(string messagePointId, string instanceId, string holder, MonitoredHumanizedLabel trigger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndDoTriggerSearch(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void SetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ResetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndRunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void RunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void EndRunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void ActivateIdleAction(string messagePointId, MonitoredHumanizedLabel activatedAction,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Info(messagePointId, message, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Trace", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Debug", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Info", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Warn", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        public void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Error", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
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
            var callInfo = DiagnosticsHelper.GetCallInfo();
            var result = LogHelper.BuildLogString(DateTime.Now, "Fatal", callInfo.ClassFullName, callInfo.MethodName, message);

            _logger.Info(result);
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
