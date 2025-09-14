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

using SymOntoClay.Common.Disposing;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SymOntoClay.Monitor.Common
{
    public interface IMonitorLogger: ISymOntoClayDisposable
    {
        /// <summary>
        /// Gets logger Id.
        /// </summary>
        string Id { get; }

        bool IsReal { get; }

        IMonitorFeatures MonitorFeatures { get; }

        KindOfLogicalSearchExplain KindOfLogicalSearchExplain { get; }

        bool EnableAddingRemovingFactLoggingInStorages { get; }

        string CreateThreadId();

        void AddEndpoint(string messagePointId,
            string endpointName,
            IReadOnlyList<int> paramsCountList,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        string CallMethod(string messagePointId, IMonitoredMethodIdentifier parameterIdentifier,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        string CallMethod(string messagePointId, IMonitoredMethodIdentifier parameterIdentifier,
            List<MonitoredHumanizedLabel> chainOfProcessInfo,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        string CallMethod(string messagePointId, string methodName,
            bool isSync,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Parameter(string messagePointId, string callMethodId, string parameterName, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Parameter(string messagePointId, string callMethodId, string parameterName, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier parameterIdentifier, object parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Parameter(string messagePointId, string callMethodId, IMonitoredMethodIdentifier parameterIdentifier, IMonitoredObject parameterValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndCallMethod(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void MethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void ActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndActionResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void HostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndHostMethodResolving(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void HostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndHostMethodActivation(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void HostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndHostMethodStarting(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void HostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndHostMethodExecution(string messagePointId, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void SystemExpr(string messagePointId, string callMethodId, string exprLabel, object exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void SystemExpr(string messagePointId, string callMethodId, string exprLabel, IMonitoredObject exprValue,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void CodeFrame(string messagePointId, string humanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void LeaveThreadExecutor(string messagePointId, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void GoBackToPrevCodeFrame(string messagePointId, int targetActionExecutionStatus, string targetActionExecutionStatusStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void StartProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void CancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void WeakCancelProcessInfo(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void CancelInstanceExecution(string messagePointId, string instanceId, Enum reasonOfChangeStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void SetExecutionCoordinatorStatus(string messagePointId, string executionCoordinatorId, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void SetProcessInfoStatus(string messagePointId, string processInfoId, MonitoredHumanizedLabel processInfo, Enum status, Enum prevStatus, List<Changer> changers, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void WaitProcessInfo(string messagePointId, string waitingProcessInfoId, MonitoredHumanizedLabel waitingProcessInfo, List<MonitoredHumanizedLabel> processes, string callMethodId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void RunLifecycleTrigger(string messagePointId, string instanceId, string holder, Enum kindOfSystemEvent,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        string DoTriggerSearch(string messagePointId, string instanceId, string holder, MonitoredHumanizedLabel trigger,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndDoTriggerSearch(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void SetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void ResetConditionalTrigger(string messagePointId, string doTriggerSearchId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void RunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndRunSetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr, 
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void RunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndRunResetExprOfConditionalTrigger(string messagePointId, string doTriggerSearchId, MonitoredHumanizedLabel expr,
            bool isSuccess, bool isPeriodic, List<List<MonitoredHumanizedLabel>> fetchedResults,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void ActivateIdleAction(string messagePointId, MonitoredHumanizedLabel activatedAction,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        ulong LogicalSearchExplain(string messagePointId, string dotStr, MonitoredHumanizedLabel query,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void AddFactOrRuleTriggerResult(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            MonitoredHumanizedLabel result,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void AddFactToLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void RemoveFactFromLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void RefreshLifeTimeInLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
            int newLifetime,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void PutFactForRemovingFromLogicalStorage(string messagePointId, MonitoredHumanizedLabel fact, MonitoredHumanizedLabel logicalStorage,
           [CallerMemberName] string memberName = "",
           [CallerFilePath] string sourceFilePath = "",
           [CallerLineNumber] int sourceLineNumber = 0);

        ulong StartThreadTask(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void StopThreadTask(string messagePointId, ulong taskId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void StartBuildPlan(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void StopBuildPlan(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        string BeginVisionFrame(string messagePointId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void BecomeInvisible(string messagePointId,
            string visionFrameId,
            string objectId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void BecomeVisible(string messagePointId,
            string visionFrameId,
            string objectId,
            float distance,
            string publicInformationHumanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void ChangedAddFocus(string messagePointId,
            string visionFrameId,
            string objectId,
            string publicInformationHumanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void ChangedRemoveFocus(string messagePointId,
            string visionFrameId,
            string objectId,
            string publicInformationHumanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void ChangedDistance(string messagePointId,
            string visionFrameId,
            string objectId,
            float distance,
            string publicInformationHumanizedStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void DumpVisionFrame(string messagePointId,
            string visionFrameId,
            List<MonitoredVisibleItem> visibleItems,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void EndVisionFrame(string messagePointId,
            string visionFrameId,            
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        //from channel
        void Output(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Trace(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Debug(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Info(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Warn(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Error(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Error(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Fatal(string messagePointId, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void Fatal(string messagePointId, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);
    }
}
