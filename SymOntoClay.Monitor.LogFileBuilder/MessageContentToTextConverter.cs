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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class MessageContentToTextConverter
    {
#if DEBUG
        private readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        public MessageContentToTextConverter(IMessageContentToTextConverterOptions options)
        {
            _options = options;
        }

        private readonly IMessageContentToTextConverterOptions _options;

        public string GetText(BaseMessage message, ILogFileCreatorContext logFileCreatorContext, string targetFileName)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var kindOfMessage = message.KindOfMessage;

            switch (kindOfMessage)
            {
                case KindOfMessage.CreateMotitorNode:
                    return GetCreateMotitorNode(message as CreateMotitorNodeMessage);

                case KindOfMessage.CreateThreadLogger:
                    return GetCreateThreadLogger(message as CreateThreadLoggerMessage);

                case KindOfMessage.CallMethod:
                    return GetCallMethod(message as CallMethodMessage);

                case KindOfMessage.Parameter:
                    return GetParameter(message as ParameterMessage);

                case KindOfMessage.EndCallMethod:
                    return GetEndCallMethod(message as EndCallMethodMessage);

                case KindOfMessage.MethodResolving:
                    return GetMethodResolving(message as MethodResolvingMessage);

                case KindOfMessage.EndMethodResolving:
                    return GetEndMethodResolving(message as EndMethodResolvingMessage);

                case KindOfMessage.ActionResolving:
                    return GetActionResolving(message as ActionResolvingMessage);

                case KindOfMessage.EndActionResolving:
                    return GetEndActionResolving(message as EndActionResolvingMessage);

                case KindOfMessage.HostMethodResolving:
                    return GetHostMethodResolving(message as HostMethodResolvingMessage);

                case KindOfMessage.EndHostMethodResolving:
                    return GetEndHostMethodResolving(message as EndHostMethodResolvingMessage);

                case KindOfMessage.HostMethodActivation:
                    return GetHostMethodActivation(message as HostMethodActivationMessage);

                case KindOfMessage.EndHostMethodActivation:
                    return GetEndHostMethodActivation(message as EndHostMethodActivationMessage);

                case KindOfMessage.HostMethodStarting:
                    return GetHostMethodStarting(message as HostMethodStartingMessage);

                case KindOfMessage.EndHostMethodStarting:
                    return GetEndHostMethodStarting(message as EndHostMethodStartingMessage);

                case KindOfMessage.HostMethodExecution:
                    return GetHostMethodExecution(message as HostMethodExecutionMessage);

                case KindOfMessage.EndHostMethodExecution:
                    return GetEndHostMethodExecution(message as EndHostMethodExecutionMessage);

                case KindOfMessage.SystemExpr:
                    return GetSystemExpr(message as SystemExprMessage);

                case KindOfMessage.CodeFrame:
                    return GetCodeFrame(message as CodeFrameMessage);

                case KindOfMessage.LeaveThreadExecutor:
                    return GetLeaveThreadExecutor(message as LeaveThreadExecutorMessage);

                case KindOfMessage.GoBackToPrevCodeFrame:
                    return GetGoBackToPrevCodeFrame(message as GoBackToPrevCodeFrameMessage);

                case KindOfMessage.StartProcessInfo:
                    return GetStartProcessInfo(message as StartProcessInfoMessage);

                case KindOfMessage.CancelProcessInfo:
                    return GetCancelProcessInfo(message as CancelProcessInfoMessage);

                case KindOfMessage.WeakCancelProcessInfo:
                    return GetWeakCancelProcessInfo(message as WeakCancelProcessInfoMessage);

                case KindOfMessage.CancelInstanceExecution:
                    return GetCancelInstanceExecution(message as CancelInstanceExecutionMessage);

                case KindOfMessage.SetExecutionCoordinatorStatus:
                    return GetSetExecutionCoordinatorStatus(message as SetExecutionCoordinatorStatusMessage);

                case KindOfMessage.SetProcessInfoStatus:
                    return GetSetProcessInfoStatus(message as SetProcessInfoStatusMessage);

                case KindOfMessage.WaitProcessInfo:
                    return GetWaitProcessInfo(message as WaitProcessInfoMessage);

                case KindOfMessage.RunLifecycleTrigger:
                    return GetRunLifecycleTrigger(message as RunLifecycleTriggerMessage);

                case KindOfMessage.DoTriggerSearch:
                    return GetDoTriggerSearch(message as DoTriggerSearchMessage);

                case KindOfMessage.EndDoTriggerSearch:
                    return GetEndDoTriggerSearch(message as EndDoTriggerSearchMessage);

                case KindOfMessage.SetConditionalTrigger:
                    return GetSetConditionalTrigger(message as SetConditionalTriggerMessage);

                case KindOfMessage.ResetConditionalTrigger:
                    return GetResetConditionalTrigger(message as ResetConditionalTriggerMessage);

                case KindOfMessage.RunSetExprOfConditionalTrigger:
                    return GetRunSetExprOfConditionalTrigger(message as RunSetExprOfConditionalTriggerMessage);

                case KindOfMessage.EndRunSetExprOfConditionalTrigger:
                    return GetEndRunSetExprOfConditionalTrigger(message as EndRunSetExprOfConditionalTriggerMessage);

                case KindOfMessage.RunResetExprOfConditionalTrigger:
                    return GetRunResetExprOfConditionalTrigger(message as RunResetExprOfConditionalTriggerMessage);

                case KindOfMessage.EndRunResetExprOfConditionalTrigger:
                    return GetEndRunResetExprOfConditionalTrigger(message as EndRunResetExprOfConditionalTriggerMessage);

                case KindOfMessage.ActivateIdleAction:
                    return GetActivateIdleAction(message as ActivateIdleActionMessage);

                case KindOfMessage.LogicalSearchExplain:
                    return GetLogicalSearchExplain(message as LogicalSearchExplainMessage, logFileCreatorContext, targetFileName);

                case KindOfMessage.AddFactOrRuleTriggerResult:
                    return GetAddFactOrRuleTriggerResult(message as AddFactOrRuleTriggerResultMessage);

                case KindOfMessage.AddFactToLogicalStorage:
                    return GetAddFactToLogicalStorage(message as AddFactToLogicalStorageMessage);

                case KindOfMessage.RemoveFactFromLogicalStorage:
                    return GetRemoveFactFromLogicalStorage(message as RemoveFactFromLogicalStorageMessage);

                case KindOfMessage.RefreshLifeTimeInLogicalStorage:
                    return GetRefreshLifeTimeInLogicalStorage(message as RefreshLifeTimeInLogicalStorageMessage);

                case KindOfMessage.PutFactForRemovingFromLogicalStorage:
                    return GetPutFactForRemovingFromLogicalStorage(message as PutFactForRemovingFromLogicalStorageMessage);

                case KindOfMessage.StartThreadTask:
                    return GetStartTask(message as StartTaskMessage);

                case KindOfMessage.StopThreadTask:
                    return GetStopTask(message as StopTaskMessage);

                case KindOfMessage.StartBuildPlan:
                    return GetStartBuildPlan(message as StartBuildPlanMessage);

                case KindOfMessage.StopBuildPlan:
                    return GetStopBuildPlan(message as StopBuildPlanMessage);

                case KindOfMessage.StartPrimitiveTask:
                    return GetStartPrimitiveTask(message as StartPrimitiveTaskMessage);

                case KindOfMessage.StopPrimitiveTask:
                    return GetStopPrimitiveTask(message as StopPrimitiveTaskMessage);

                case KindOfMessage.PlanFrame:
                    return GetFrame(message as PlanFrameMessage);

                case KindOfMessage.LeaveTasksExecutor:
                    return GetLeaveTasksExecutor(message as LeaveTasksExecutorMessage);

                case KindOfMessage.Output:
                    return GetOutput(message as OutputMessage);

                case KindOfMessage.Trace:
                    return GetTrace(message as TraceMessage, logFileCreatorContext);

                case KindOfMessage.Debug:
                    return GetDebug(message as DebugMessage, logFileCreatorContext);

                case KindOfMessage.Info:
                    return GetInfoMessage(message as InfoMessage, logFileCreatorContext);

                case KindOfMessage.Warn:
                    return GetWarn(message as WarnMessage, logFileCreatorContext);

                case KindOfMessage.Error:
                    return GetError(message as ErrorMessage, logFileCreatorContext);

                case KindOfMessage.Fatal:
                    return GetFatal(message as FatalMessage, logFileCreatorContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfMessage), kindOfMessage, null);
            }
        }

        private string GetCreateMotitorNode(CreateMotitorNodeMessage message)
        {
            return $"MotitorNode '{message.NodeId}' has been created";
        }

        private string GetCreateThreadLogger(CreateThreadLoggerMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"MotitorNode '{message.NodeId}' has created ThreadLogger '{message.ThreadId}'");

            if (!string.IsNullOrWhiteSpace(message.ParentThreadId))
            {
                sb.Append($" with ParentThreadId '{message.ParentThreadId}'");
            }

            return sb.ToString();
        }

        private string GetMonitoredHumanizedMethodArgument(MonitoredHumanizedMethodArgument argument)
        {
#if DEBUG
            //_globalLogger.Info($"argument = {argument}");
#endif

            var sb = new StringBuilder(argument.HumanizedStr);

            if(_options.EnableTypesListOfMethodSignatureArguments)
            {
                var typesList = argument.TypesList;

                if (typesList?.Any() ?? false)
                {
                    sb.Append(": ");

                    if (typesList.Count == 1)
                    {
                        sb.Append(typesList.Single());
                    }
                    else
                    {
                        sb.Append($"({string.Join("|", typesList)})");
                    }
                }
            }

            if(_options.EnableDefaultValueOfMethodSignatureArguments)
            {
                var defaultValue = argument.DefaultValue;

                if (defaultValue != null)
                {
                    sb.Append($" = {GetMonitoredHumanizedMethodParameterValue(defaultValue)}");
                }
            }

#if DEBUG
            //_globalLogger.Info($"sb = {sb}");
#endif

            return sb.ToString();
        }

        private string GetMonitoredHumanizedMethodParameterValue(MonitoredHumanizedMethodParameterValue value)
        {
#if DEBUG
            //_globalLogger.Info($"value = {value}");
#endif

            var nameHumanizedStr = value.NameHumanizedStr;
            var valueHumanizedStr = value.ValueHumanizedStr;

            if(string.IsNullOrWhiteSpace(nameHumanizedStr))
            {
                return valueHumanizedStr;
            }

            return $"{nameHumanizedStr} = {valueHumanizedStr}";
        }

        private string GetMonitoredHumanizedLabel(MonitoredHumanizedLabel label)
        {
#if DEBUG
            //_globalLogger.Info($"label = {label}");
#endif

            if(label == null)
            {
                return "NULL";
            }

            var sb = new StringBuilder();

            var kindOfCodeItemDescriptor = label.KindOfCodeItemDescriptor;

            if (!string.IsNullOrWhiteSpace(kindOfCodeItemDescriptor))
            {
                if(kindOfCodeItemDescriptor != "fun" 
                    && kindOfCodeItemDescriptor != "op")
                {
#if DEBUG
                    _globalLogger.Info($"label = {label}");
#endif

                    throw new NotImplementedException("4EC39690-30B7-4D88-8CC8-B7A8328851D6");
                }
            }

            sb.Append($" {label.Label}");

            if(_options.EnableMethodSignatureArguments)
            {
                var signatures = label.Signatures;

                if (signatures != null)
                {
#if DEBUG
                    //_globalLogger.Info($"label = {label}");
#endif

                    sb.Append("(");

                    var signaturesStrList = new List<string>();

                    foreach (var item in signatures)
                    {
                        signaturesStrList.Add(GetMonitoredHumanizedMethodArgument(item));
                    }

                    sb.Append(")");
                }
            }

            if(_options.EnableCallMethodIdOfMethodLabel)
            {
                var callMethodId = label.CallMethodId;

                if (!string.IsNullOrWhiteSpace(callMethodId))
                {
                    sb.Append($" <CallMethodId: {callMethodId}>");
                }
            }

            if(_options.EnablePassedVauesOfMethodLabel)
            {
                var values = label.Values;

                if (values?.Any() ?? false)
                {
#if DEBUG
                    //_globalLogger.Info($"label = {label}");
#endif

                    var valuesStrList = new List<string>();

                    foreach (var item in values)
                    {
                        valuesStrList.Add(GetMonitoredHumanizedMethodParameterValue(item));
                    }

                    sb.Append($" <{string.Join(", ", valuesStrList)}>");
                }
            }

            return sb.ToString();
        }

        private string GetMonitoredHumanizedLabel(MonitoredHumanizedLabel label, string altLabel)
        {
            if (string.IsNullOrWhiteSpace(altLabel))
            {
                return GetMonitoredHumanizedLabel(label);
            }

            return altLabel;
        }

        private string GetCallMethod(CallMethodMessage message)
        {
            var labelsStrList = new List<string>();

            if(_options.EnableChainOfProcessInfo)
            {
                var chainOfProcessInfo = message.ChainOfProcessInfo;

                if (!chainOfProcessInfo.IsNullOrEmpty())
                {
#if DEBUG
                    //_globalLogger.Info($"message = {message}");
#endif

                    foreach (var item in chainOfProcessInfo)
                    {
                        labelsStrList.Add(GetMonitoredHumanizedLabel(item));
                    }
                }
            }

            labelsStrList.Add(GetMonitoredHumanizedLabel(message.MethodLabel, message.AltMethodName));

            return $"<{message.CallMethodId}> [{(message.IsSync ? "sync" : "async")}] {string.Join(" -> ", labelsStrList)}";
        }

        private string GetParameter(ParameterMessage message)
        {
            var tmpResult = $"Parameter of <{message.CallMethodId}>: '{GetMonitoredHumanizedLabel(message.Label, message.AltLabel)}' = {message.HumanizedString}";//{ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(message.Base64Content, message.TypeName)}

#if DEBUG
            //_globalLogger.Info($"tmpResult = {tmpResult}");
#endif

            return tmpResult;
        }

        private string GetEndCallMethod(EndCallMethodMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetMethodResolving(MethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetEndMethodResolving(EndMethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetActionResolving(ActionResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetEndActionResolving(EndActionResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetHostMethodResolving(HostMethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetEndHostMethodResolving(EndHostMethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetHostMethodActivation(HostMethodActivationMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetEndHostMethodActivation(EndHostMethodActivationMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetHostMethodStarting(HostMethodStartingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetEndHostMethodStarting(EndHostMethodStartingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetHostMethodExecution(HostMethodExecutionMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetEndHostMethodExecution(EndHostMethodExecutionMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private string GetSystemExpr(SystemExprMessage message)
        {
            var tmpResult = $"Expression of <{message.CallMethodId}>: '{GetMonitoredHumanizedLabel(message.Label, message.AltLabel)}' = {message.HumanizedString}";//{ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(message.Base64Content, message.TypeName)}

#if DEBUG
            //_globalLogger.Info($"tmpResult = {tmpResult}");
#endif

            return tmpResult;
        }

        private string GetCodeFrame(CodeFrameMessage message)
        {
#if DEBUG
            _globalLogger.Info($"tmpResult = {message.HumanizedStr}");
#endif

            return $"\n{message.HumanizedStr}\n";
        }

        private string GetLeaveThreadExecutor(LeaveThreadExecutorMessage message)
        {
            return string.Empty;
        }

        public string GetGoBackToPrevCodeFrame(GoBackToPrevCodeFrameMessage message)
        {
            return message.TargetActionExecutionStatusStr;
        }

        private string GetChangers(List<Changer> changers)
        {
#if DEBUG
            //_globalLogger.Info($"changers = {changers.WriteListToString()}");
#endif

            return $"[{string.Join(", ", changers.Select(p => $"{p.KindOfChanger}: {p.Id}"))}]";
        }

        private string GetProcessInfo(MonitoredHumanizedLabel processInfo, string processInfoId)
        {
            if(processInfo == null)
            {
                return processInfoId;
            }

            return GetMonitoredHumanizedLabel(processInfo);
        }

        private string GetStartProcessInfo(StartProcessInfoMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"ProcessInfo [{GetProcessInfo(message.ProcessInfo, message.ProcessInfoId)}] has been started";
        }

        public string GetCancelProcessInfo(CancelProcessInfoMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"ProcessInfo [GetProcessInfo(message.ProcessInfo, message.CancelledObjId)] has been cancelled");

            if(!message.Changers.IsNullOrEmpty())
            {
                sb.Append($" by {GetChangers(message.Changers)}");
            }

            if(!string.IsNullOrWhiteSpace(message.CallMethodId))
            {
                sb.Append($" when CallMethodId = {message.CallMethodId}");
            }

            return sb.ToString();
        }

        public string GetWeakCancelProcessInfo(WeakCancelProcessInfoMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"ProcessInfo [{GetProcessInfo(message.ProcessInfo, message.CancelledObjId)}] has been weak cancelled");

            if (!message.Changers.IsNullOrEmpty())
            {
                sb.Append($" by {GetChangers(message.Changers)}");
            }

            if (!string.IsNullOrWhiteSpace(message.CallMethodId))
            {
                sb.Append($" when CallMethodId = {message.CallMethodId}");
            }

            return sb.ToString();
        }

        public string GetCancelInstanceExecution(CancelInstanceExecutionMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"Instance [{message.CancelledObjId}] was cancelled");

            if (!message.Changers.IsNullOrEmpty())
            {
                sb.Append($" by {GetChangers(message.Changers)}");
            }

            if (!string.IsNullOrWhiteSpace(message.CallMethodId))
            {
                sb.Append($" when CallMethodId = {message.CallMethodId}");
            }

            return sb.ToString();
        }

        public string GetSetExecutionCoordinatorStatus(SetExecutionCoordinatorStatusMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"Set status {message.StatusStr}");

            if(message.PrevStatus != 0)
            {
                sb.Append($" ({message.PrevStatusStr} -> {message.StatusStr})");
            }

            sb.Append($" for execution coordinator {message.ObjId}");

            if (!message.Changers.IsNullOrEmpty())
            {
                sb.Append($" by {GetChangers(message.Changers)}");
            }

            if (!string.IsNullOrWhiteSpace(message.CallMethodId))
            {
                sb.Append($" when CallMethodId = {message.CallMethodId}");
            }
             
            return sb.ToString();
        }

        public string GetSetProcessInfoStatus(SetProcessInfoStatusMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"Set status {message.StatusStr}");

            if (message.PrevStatus != 0)
            {
                sb.Append($" ({message.PrevStatusStr} -> {message.StatusStr})");
            }

            sb.Append($" for process info [{GetProcessInfo(message.ProcessInfo, message.ObjId)}]");

            if (!message.Changers.IsNullOrEmpty())
            {
                sb.Append($" by {GetChangers(message.Changers)}");
            }

            if (!string.IsNullOrWhiteSpace(message.CallMethodId))
            {
                sb.Append($" when CallMethodId = {message.CallMethodId}");
            }

            return sb.ToString();
        }

        private string GetWaitProcessInfo(WaitProcessInfoMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"ProcessInfo [{GetProcessInfo(message.WaitingProcessInfo, message.WaitingProcessInfoId)}]");

            if (!string.IsNullOrWhiteSpace(message.CallMethodId))
            {
                sb.Append($" when CallMethodId = {message.CallMethodId}");
            }

            sb.AppendLine($" is waiting for:");

            foreach(var item in message.Processes)
            {
                sb.AppendLine(GetMonitoredHumanizedLabel(item));
            }

            return sb.ToString();
        }

        private string GetRunLifecycleTrigger(RunLifecycleTriggerMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder(message.StatusStr);

            if(!string.IsNullOrWhiteSpace(message.InstanceId))
            {
                sb.Append($" Instance: {message.InstanceId}");
            }

            if (!string.IsNullOrWhiteSpace(message.Holder))
            {
                sb.Append($" Holder: {message.Holder}");
            }

            return sb.ToString();
        }

        private string GetDoTriggerSearch(DoTriggerSearchMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"{message.TriggerLabel.Label} <{message.DoTriggerSearchId}>");

            if (!string.IsNullOrWhiteSpace(message.InstanceId))
            {
                sb.Append($" Instance: {message.InstanceId}");
            }

            if (!string.IsNullOrWhiteSpace(message.Holder))
            {
                sb.Append($" Holder: {message.Holder}");
            }

            return sb.ToString();
        }

        private string GetEndDoTriggerSearch(EndDoTriggerSearchMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"<{message.DoTriggerSearchId}>";
        }

        private string GetSetConditionalTrigger(SetConditionalTriggerMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"<{message.DoTriggerSearchId}>";
        }

        private string GetResetConditionalTrigger(ResetConditionalTriggerMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"<{message.DoTriggerSearchId}>";
        }

        private string GetRunSetExprOfConditionalTrigger(RunSetExprOfConditionalTriggerMessage message)
        {
            return GetBaseRunExprOfConditionalTriggerMessage(message);
        }

        private string GetBaseRunExprOfConditionalTriggerMessage(BaseRunExprOfConditionalTriggerMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{message.ExprLabel.Label} <{message.DoTriggerSearchId}>";
        }

        private string GetEndRunSetExprOfConditionalTrigger(EndRunSetExprOfConditionalTriggerMessage message)
        {
            return GetBaseEndRunExprOfConditionalTriggerMessage(message);
        }

        private string GetBaseEndRunExprOfConditionalTriggerMessage(BaseEndRunExprOfConditionalTriggerMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"{message.ExprLabel.Label} <{message.DoTriggerSearchId}> {(message.IsSuccess ? "success" : "failed")}");

            if(message.IsPeriodic)
            {
                sb.Append(" periodic");
            }

            var fetchedResults = message.FetchedResults;

            if (!fetchedResults.IsNullOrEmpty())
            {
#if DEBUG
                //_globalLogger.Info($"message = {message}");
#endif

                sb.AppendLine();
                sb.AppendLine("Fetched results:");

                foreach(var result in fetchedResults)
                {
                    var strList = new List<string>();

                    foreach (var item in result)
                    {
                        strList.Add(item.Label);
                    }

                    sb.AppendLine(string.Join("; ", strList));
                }
            }

            return sb.ToString();
        }

        private string GetRunResetExprOfConditionalTrigger(RunResetExprOfConditionalTriggerMessage message)
        {
            return GetBaseRunExprOfConditionalTriggerMessage(message);
        }

        private string GetEndRunResetExprOfConditionalTrigger(EndRunResetExprOfConditionalTriggerMessage message)
        {
            return GetBaseEndRunExprOfConditionalTriggerMessage(message);
        }

        private string GetActivateIdleAction(ActivateIdleActionMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return GetMonitoredHumanizedLabel(message.ActivatedAction);
        }

        private string GetAddFactOrRuleTriggerResult(AddFactOrRuleTriggerResultMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)} Result: {GetMonitoredHumanizedLabel(message.Result)}";
        }

        private string GetAddFactToLogicalStorage(AddFactToLogicalStorageMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)}";
        }

        private string GetRemoveFactFromLogicalStorage(RemoveFactFromLogicalStorageMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)}";
        }

        private string GetRefreshLifeTimeInLogicalStorage(RefreshLifeTimeInLogicalStorageMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)} NewLifetime: {message.NewLifetime}";
        }

        private string GetPutFactForRemovingFromLogicalStorage(PutFactForRemovingFromLogicalStorageMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)}";
        }

        private string GetStartTask(StartTaskMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            _threadTasksTime[message.TaskId] = message.DateTimeStamp;

            return $"{message.TaskId} ({message.TasksCount} tasks)";
        }

        private Dictionary<ulong, DateTime> _threadTasksTime = new Dictionary<ulong, DateTime>();

        private string GetStopTask(StopTaskMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{message.TaskId} {(_threadTasksTime.TryGetValue(message.TaskId, out var date) ? message.DateTimeStamp.Subtract(date).ToString() : string.Empty)} ({message.TasksCount} tasks)";
        }

        private string GetStartBuildPlan(StartBuildPlanMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return string.Empty;
        }

        private string GetStopBuildPlan(StopBuildPlanMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return string.Empty;
        }

        private Dictionary<ulong, DateTime> _primitiveTasksTime = new Dictionary<ulong, DateTime>();

        private string GetStartPrimitiveTask(StartPrimitiveTaskMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            _primitiveTasksTime[message.TaskId] = message.DateTimeStamp;

            return message.TaskId.ToString();
        }

        private string GetStopPrimitiveTask(StopPrimitiveTaskMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{message.TaskId} {(_primitiveTasksTime.TryGetValue(message.TaskId, out var date) ? message.DateTimeStamp.Subtract(date).ToString() : string.Empty)}";
        }

        private string GetFrame(PlanFrameMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"\n{message.HumanizedStr}\n";
        }

        private string GetLeaveTasksExecutor(LeaveTasksExecutorMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return string.Empty;
        }

        private string GetOutput(OutputMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private string GetLogicalSearchExplain(LogicalSearchExplainMessage message, ILogFileCreatorContext logFileCreatorContext, string targetFileName)
        {
#if DEBUG
            //_globalLogger.Info($"targetFileName = {targetFileName}");
            //_globalLogger.Info($"message = {message}");
#endif

            var bytes = Convert.FromBase64String(message.DotContent);
            var dotStr = Encoding.UTF8.GetString(bytes);

#if DEBUG
            //_globalLogger.Info($"dotStr = {dotStr}");
#endif

            var fileName = logFileCreatorContext.ConvertDotStrToImg(dotStr, targetFileName);

#if DEBUG
            //_globalLogger.Info($"fileName = {fileName}");
#endif

            return $"{GetMonitoredHumanizedLabel(message.Query)}: {logFileCreatorContext.CreateImgLink(fileName.AbsoluteName, fileName.RelativeName)}";
        }

        private string GetTrace(TraceMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(message.Message);
        }

        private string GetDebug(DebugMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(message.Message);
        }

        private string GetInfoMessage(InfoMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(message.Message);
        }

        private string GetWarn(WarnMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(message.Message);
        }

        private string GetError(ErrorMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(message.Message);
        }

        private string GetFatal(FatalMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(message.Message);
        }
    }
}
