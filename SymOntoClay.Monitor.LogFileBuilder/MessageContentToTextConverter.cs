/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
                case KindOfMessage.CreateMonitorNode:
                    return GetCreateMonitorNode(message as CreateMonitorNodeMessage, logFileCreatorContext);

                case KindOfMessage.CreateThreadLogger:
                    return GetCreateThreadLogger(message as CreateThreadLoggerMessage, logFileCreatorContext);

                case KindOfMessage.AddEndpoint:
                    return GetAddEndpoint(message as AddEndpointMessage, logFileCreatorContext);

                case KindOfMessage.CallMethod:
                    return GetCallMethod(message as CallMethodMessage, logFileCreatorContext);

                case KindOfMessage.Parameter:
                    return GetParameter(message as ParameterMessage, logFileCreatorContext);

                case KindOfMessage.EndCallMethod:
                    return GetEndCallMethod(message as EndCallMethodMessage, logFileCreatorContext);

                case KindOfMessage.MethodResolving:
                    return GetMethodResolving(message as MethodResolvingMessage, logFileCreatorContext);

                case KindOfMessage.EndMethodResolving:
                    return GetEndMethodResolving(message as EndMethodResolvingMessage, logFileCreatorContext);

                case KindOfMessage.ActionResolving:
                    return GetActionResolving(message as ActionResolvingMessage, logFileCreatorContext);

                case KindOfMessage.EndActionResolving:
                    return GetEndActionResolving(message as EndActionResolvingMessage, logFileCreatorContext);

                case KindOfMessage.HostMethodResolving:
                    return GetHostMethodResolving(message as HostMethodResolvingMessage, logFileCreatorContext);

                case KindOfMessage.EndHostMethodResolving:
                    return GetEndHostMethodResolving(message as EndHostMethodResolvingMessage, logFileCreatorContext);

                case KindOfMessage.HostMethodActivation:
                    return GetHostMethodActivation(message as HostMethodActivationMessage, logFileCreatorContext);

                case KindOfMessage.EndHostMethodActivation:
                    return GetEndHostMethodActivation(message as EndHostMethodActivationMessage, logFileCreatorContext);

                case KindOfMessage.HostMethodStarting:
                    return GetHostMethodStarting(message as HostMethodStartingMessage, logFileCreatorContext);

                case KindOfMessage.EndHostMethodStarting:
                    return GetEndHostMethodStarting(message as EndHostMethodStartingMessage, logFileCreatorContext);

                case KindOfMessage.HostMethodExecution:
                    return GetHostMethodExecution(message as HostMethodExecutionMessage, logFileCreatorContext);

                case KindOfMessage.EndHostMethodExecution:
                    return GetEndHostMethodExecution(message as EndHostMethodExecutionMessage, logFileCreatorContext);

                case KindOfMessage.SystemExpr:
                    return GetSystemExpr(message as SystemExprMessage, logFileCreatorContext);

                case KindOfMessage.CodeFrame:
                    return GetCodeFrame(message as CodeFrameMessage, logFileCreatorContext);

                case KindOfMessage.LeaveThreadExecutor:
                    return GetLeaveThreadExecutor(message as LeaveThreadExecutorMessage, logFileCreatorContext);

                case KindOfMessage.GoBackToPrevCodeFrame:
                    return GetGoBackToPrevCodeFrame(message as GoBackToPrevCodeFrameMessage, logFileCreatorContext);

                case KindOfMessage.StartProcessInfo:
                    return GetStartProcessInfo(message as StartProcessInfoMessage, logFileCreatorContext);

                case KindOfMessage.CancelProcessInfo:
                    return GetCancelProcessInfo(message as CancelProcessInfoMessage, logFileCreatorContext);

                case KindOfMessage.WeakCancelProcessInfo:
                    return GetWeakCancelProcessInfo(message as WeakCancelProcessInfoMessage, logFileCreatorContext);

                case KindOfMessage.CancelInstanceExecution:
                    return GetCancelInstanceExecution(message as CancelInstanceExecutionMessage, logFileCreatorContext);

                case KindOfMessage.SetExecutionCoordinatorStatus:
                    return GetSetExecutionCoordinatorStatus(message as SetExecutionCoordinatorStatusMessage, logFileCreatorContext);

                case KindOfMessage.SetProcessInfoStatus:
                    return GetSetProcessInfoStatus(message as SetProcessInfoStatusMessage, logFileCreatorContext);

                case KindOfMessage.WaitProcessInfo:
                    return GetWaitProcessInfo(message as WaitProcessInfoMessage, logFileCreatorContext);

                case KindOfMessage.RunLifecycleTrigger:
                    return GetRunLifecycleTrigger(message as RunLifecycleTriggerMessage, logFileCreatorContext);

                case KindOfMessage.DoTriggerSearch:
                    return GetDoTriggerSearch(message as DoTriggerSearchMessage, logFileCreatorContext);

                case KindOfMessage.EndDoTriggerSearch:
                    return GetEndDoTriggerSearch(message as EndDoTriggerSearchMessage, logFileCreatorContext);

                case KindOfMessage.SetConditionalTrigger:
                    return GetSetConditionalTrigger(message as SetConditionalTriggerMessage, logFileCreatorContext);

                case KindOfMessage.ResetConditionalTrigger:
                    return GetResetConditionalTrigger(message as ResetConditionalTriggerMessage, logFileCreatorContext);

                case KindOfMessage.RunSetExprOfConditionalTrigger:
                    return GetRunSetExprOfConditionalTrigger(message as RunSetExprOfConditionalTriggerMessage, logFileCreatorContext);

                case KindOfMessage.EndRunSetExprOfConditionalTrigger:
                    return GetEndRunSetExprOfConditionalTrigger(message as EndRunSetExprOfConditionalTriggerMessage, logFileCreatorContext);

                case KindOfMessage.RunResetExprOfConditionalTrigger:
                    return GetRunResetExprOfConditionalTrigger(message as RunResetExprOfConditionalTriggerMessage, logFileCreatorContext);

                case KindOfMessage.EndRunResetExprOfConditionalTrigger:
                    return GetEndRunResetExprOfConditionalTrigger(message as EndRunResetExprOfConditionalTriggerMessage, logFileCreatorContext);

                case KindOfMessage.ActivateIdleAction:
                    return GetActivateIdleAction(message as ActivateIdleActionMessage, logFileCreatorContext);

                case KindOfMessage.LogicalSearchExplain:
                    return GetLogicalSearchExplain(message as LogicalSearchExplainMessage, logFileCreatorContext, targetFileName);

                case KindOfMessage.AddFactOrRuleTriggerResult:
                    return GetAddFactOrRuleTriggerResult(message as AddFactOrRuleTriggerResultMessage, logFileCreatorContext);

                case KindOfMessage.AddFactToLogicalStorage:
                    return GetAddFactToLogicalStorage(message as AddFactToLogicalStorageMessage, logFileCreatorContext);

                case KindOfMessage.RemoveFactFromLogicalStorage:
                    return GetRemoveFactFromLogicalStorage(message as RemoveFactFromLogicalStorageMessage, logFileCreatorContext);

                case KindOfMessage.RefreshLifeTimeInLogicalStorage:
                    return GetRefreshLifeTimeInLogicalStorage(message as RefreshLifeTimeInLogicalStorageMessage, logFileCreatorContext);

                case KindOfMessage.PutFactForRemovingFromLogicalStorage:
                    return GetPutFactForRemovingFromLogicalStorage(message as PutFactForRemovingFromLogicalStorageMessage, logFileCreatorContext);

                case KindOfMessage.StartThreadTask:
                    return GetStartTask(message as StartTaskMessage, logFileCreatorContext);

                case KindOfMessage.StopThreadTask:
                    return GetStopTask(message as StopTaskMessage, logFileCreatorContext);

                case KindOfMessage.StartBuildPlan:
                    return GetStartBuildPlan(message as StartBuildPlanMessage, logFileCreatorContext);

                case KindOfMessage.StopBuildPlan:
                    return GetStopBuildPlan(message as StopBuildPlanMessage, logFileCreatorContext);

                case KindOfMessage.BeginVisionFrame:
                    return GetBeginVisionFrame(message as BeginVisionFrameMessage, logFileCreatorContext);

                case KindOfMessage.BecomeInvisible:
                    return GetBecomeInvisible(message as BecomeInvisibleMessage, logFileCreatorContext);

                case KindOfMessage.BecomeVisible:
                    return GetBecomeVisible(message as BecomeVisibleMessage, logFileCreatorContext);

                case KindOfMessage.ChangedAddFocus:
                    return GetChangedAddFocus(message as ChangedAddFocusMessage, logFileCreatorContext);

                case KindOfMessage.ChangedRemoveFocus:
                    return GetChangedRemoveFocus(message as ChangedRemoveFocusMessage, logFileCreatorContext);

                case KindOfMessage.ChangedDistance:
                    return GetChangedDistance(message as ChangedDistanceMessage, logFileCreatorContext);

                case KindOfMessage.DumpVisionFrame:
                    return GetDumpVisionFrame(message as DumpVisionFrameMessage, logFileCreatorContext);

                case KindOfMessage.EndVisionFrame:
                    return GetEndVisionFrame(message as EndVisionFrameMessage, logFileCreatorContext);

                case KindOfMessage.Output:
                    return GetOutput(message as OutputMessage, logFileCreatorContext);

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

        private string GetCreateMonitorNode(CreateMonitorNodeMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return logFileCreatorContext.NormalizeAsHtml($"MonitorNode '{message.NodeId}' has been created");
        }

        private string GetCreateThreadLogger(CreateThreadLoggerMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder($"MonitorNode '{message.NodeId}' has created ThreadLogger '{message.ThreadId}'");

            if (!string.IsNullOrWhiteSpace(message.ParentThreadId))
            {
                sb.Append($" with ParentThreadId '{message.ParentThreadId}'");
            }

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
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

            if(_options.EnablePassedValuesOfMethodLabel)
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

        private string GetAddEndpoint(AddEndpointMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"'{message.EndpointName}'; Params count: [{string.Join(", ", message.ParamsCountList)}]");
        }

        private Dictionary<string, CallMethodMessage> _callMethodMessagesDict = new Dictionary<string, CallMethodMessage>();
        private Dictionary<string, string> _methodLabelsDict = new Dictionary<string, string>();

        private string GetCallMethod(CallMethodMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            _callMethodMessagesDict[message.CallMethodId] = message;

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

            return logFileCreatorContext.NormalizeAsHtml($"<{message.CallMethodId}> [{(message.IsSync ? "sync" : "async")}] {string.Join(" -> ", labelsStrList)}");
        }

        private string GetMethodLabel(string callMethodId)
        {
            if(_methodLabelsDict.TryGetValue(callMethodId, out var result))
            {
                return result;
            }

            var callMethodMessage = _callMethodMessagesDict[callMethodId];

            var methodLabel = $"[{(callMethodMessage.IsSync ? "sync" : "async")}] {GetMonitoredHumanizedLabel(callMethodMessage.MethodLabel, callMethodMessage.AltMethodName)}";

            _methodLabelsDict[callMethodId] = methodLabel;

            return methodLabel;
        }

        private string GetParameter(ParameterMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var callMethodId = message.CallMethodId;

            var tmpResult = logFileCreatorContext.NormalizeAsHtml($"Parameter of <{callMethodId} ({GetMethodLabel(callMethodId)})>: '{GetMonitoredHumanizedLabel(message.Label, message.AltLabel)}' = {message.HumanizedString}");//{ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(message.Base64Content, message.TypeName)}

#if DEBUG
            //_globalLogger.Info($"tmpResult = {tmpResult}");
#endif

            return tmpResult;
        }

        private string GetEndCallMethod(EndCallMethodMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var callMethodId = message.CallMethodId;

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetMethodResolving(MethodResolvingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var callMethodId = message.CallMethodId;

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetEndMethodResolving(EndMethodResolvingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var callMethodId = message.CallMethodId;

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetActionResolving(ActionResolvingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var callMethodId = message.CallMethodId;

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetEndActionResolving(EndActionResolvingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var callMethodId = message.CallMethodId;

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetHostMethodResolving(HostMethodResolvingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetEndHostMethodResolving(EndHostMethodResolvingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetHostMethodActivation(HostMethodActivationMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetEndHostMethodActivation(EndHostMethodActivationMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetHostMethodStarting(HostMethodStartingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetEndHostMethodStarting(EndHostMethodStartingMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetHostMethodExecution(HostMethodExecutionMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetEndHostMethodExecution(EndHostMethodExecutionMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var callMethodId = message.CallMethodId;

#if DEBUG
            //_globalLogger.Info($"GetMethodLabel(callMethodId) = {GetMethodLabel(callMethodId)}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{callMethodId} ({GetMethodLabel(callMethodId)})>");
        }

        private string GetSystemExpr(SystemExprMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            var callMethodId = message.CallMethodId;

            var tmpResult = logFileCreatorContext.NormalizeAsHtml($"Expression of <{callMethodId} ({GetMethodLabel(callMethodId)})>: '{GetMonitoredHumanizedLabel(message.Label, message.AltLabel)}' = {message.HumanizedString}");//{ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(message.Base64Content, message.TypeName)}

#if DEBUG
            //_globalLogger.Info($"tmpResult = {tmpResult}");
#endif

            return tmpResult;
        }

        private string GetCodeFrame(CodeFrameMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            _globalLogger.Info($"tmpResult = {message.HumanizedStr}");
#endif

            return $"\n{logFileCreatorContext.NormalizeAsHtml(message.HumanizedStr)}\n";
        }

        private string GetLeaveThreadExecutor(LeaveThreadExecutorMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return string.Empty;
        }

        public string GetGoBackToPrevCodeFrame(GoBackToPrevCodeFrameMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return logFileCreatorContext.NormalizeAsHtml(message.TargetActionExecutionStatusStr);
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

        private string GetStartProcessInfo(StartProcessInfoMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"ProcessInfo [{GetProcessInfo(message.ProcessInfo, message.ProcessInfoId)}] has been started");
        }

        public string GetCancelProcessInfo(CancelProcessInfoMessage message, ILogFileCreatorContext logFileCreatorContext)
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

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        public string GetWeakCancelProcessInfo(WeakCancelProcessInfoMessage message, ILogFileCreatorContext logFileCreatorContext)
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

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        public string GetCancelInstanceExecution(CancelInstanceExecutionMessage message, ILogFileCreatorContext logFileCreatorContext)
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

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        public string GetSetExecutionCoordinatorStatus(SetExecutionCoordinatorStatusMessage message, ILogFileCreatorContext logFileCreatorContext)
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
             
            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetSetProcessInfoStatus(SetProcessInfoStatusMessage message, ILogFileCreatorContext logFileCreatorContext)
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

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetWaitProcessInfo(WaitProcessInfoMessage message, ILogFileCreatorContext logFileCreatorContext)
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

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetRunLifecycleTrigger(RunLifecycleTriggerMessage message, ILogFileCreatorContext logFileCreatorContext)
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

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetDoTriggerSearch(DoTriggerSearchMessage message, ILogFileCreatorContext logFileCreatorContext)
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

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetEndDoTriggerSearch(EndDoTriggerSearchMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{message.DoTriggerSearchId}>");
        }

        private string GetSetConditionalTrigger(SetConditionalTriggerMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{message.DoTriggerSearchId}>");
        }

        private string GetResetConditionalTrigger(ResetConditionalTriggerMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{message.DoTriggerSearchId}>");
        }

        private string GetRunSetExprOfConditionalTrigger(RunSetExprOfConditionalTriggerMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return logFileCreatorContext.NormalizeAsHtml(GetBaseRunExprOfConditionalTriggerMessage(message));
        }

        private string GetBaseRunExprOfConditionalTriggerMessage(BaseRunExprOfConditionalTriggerMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return $"{message.ExprLabel.Label} <{message.DoTriggerSearchId}>";
        }

        private string GetEndRunSetExprOfConditionalTrigger(EndRunSetExprOfConditionalTriggerMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return logFileCreatorContext.NormalizeAsHtml(GetBaseEndRunExprOfConditionalTriggerMessage(message));
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

        private string GetRunResetExprOfConditionalTrigger(RunResetExprOfConditionalTriggerMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return logFileCreatorContext.NormalizeAsHtml(GetBaseRunExprOfConditionalTriggerMessage(message));
        }

        private string GetEndRunResetExprOfConditionalTrigger(EndRunResetExprOfConditionalTriggerMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
            return logFileCreatorContext.NormalizeAsHtml(GetBaseEndRunExprOfConditionalTriggerMessage(message));
        }

        private string GetActivateIdleAction(ActivateIdleActionMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml(GetMonitoredHumanizedLabel(message.ActivatedAction));
        }

        private string GetAddFactOrRuleTriggerResult(AddFactOrRuleTriggerResultMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)} Result: {GetMonitoredHumanizedLabel(message.Result)}");
        }

        private string GetAddFactToLogicalStorage(AddFactToLogicalStorageMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)}");
        }

        private string GetRemoveFactFromLogicalStorage(RemoveFactFromLogicalStorageMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)}");
        }

        private string GetRefreshLifeTimeInLogicalStorage(RefreshLifeTimeInLogicalStorageMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)} NewLifetime: {message.NewLifetime}");
        }

        private string GetPutFactForRemovingFromLogicalStorage(PutFactForRemovingFromLogicalStorageMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"{GetMonitoredHumanizedLabel(message.LogicalStorage)}: {GetMonitoredHumanizedLabel(message.Fact)}");
        }

        private string GetStartTask(StartTaskMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            _threadTasksTime[message.TaskId] = message.DateTimeStamp;

            return logFileCreatorContext.NormalizeAsHtml($"{message.TaskId} ({message.TasksCount} tasks)");
        }

        private Dictionary<ulong, DateTime> _threadTasksTime = new Dictionary<ulong, DateTime>();

        private string GetStopTask(StopTaskMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"{message.TaskId} {(_threadTasksTime.TryGetValue(message.TaskId, out var date) ? message.DateTimeStamp.Subtract(date).ToString() : string.Empty)} ({message.TasksCount} tasks)");
        }

        private string GetStartBuildPlan(StartBuildPlanMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return string.Empty;
        }

        private string GetStopBuildPlan(StopBuildPlanMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return string.Empty;
        }

        private string GetBeginVisionFrame(BeginVisionFrameMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{message.VisionFrameId}>");
        }

        private string GetBecomeInvisible(BecomeInvisibleMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{message.VisionFrameId}> : {message.ObjectId}");
        }

        private string GetBecomeVisible(BecomeVisibleMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder();
            sb.AppendLine($"<{message.VisionFrameId}> : {message.ObjectId}");
            sb.AppendLine($"Distance: {message.Distance}");

            var publicInformationHumanizedStr = message.PublicInformationHumanizedStr;

            if(!string.IsNullOrWhiteSpace(publicInformationHumanizedStr))
            {
                sb.AppendLine("Begin Public facts");
                sb.AppendLine(publicInformationHumanizedStr);
                sb.AppendLine("End Public facts");
            }

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetChangedAddFocus(ChangedAddFocusMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder();
            sb.AppendLine($"<{message.VisionFrameId}> : {message.ObjectId}");

            var publicInformationHumanizedStr = message.PublicInformationHumanizedStr;

            if (!string.IsNullOrWhiteSpace(publicInformationHumanizedStr))
            {
                sb.AppendLine("Begin Public facts");
                sb.AppendLine(publicInformationHumanizedStr);
                sb.AppendLine("End Public facts");
            }

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetChangedRemoveFocus(ChangedRemoveFocusMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder();
            sb.AppendLine($"<{message.VisionFrameId}> : {message.ObjectId}");

            var publicInformationHumanizedStr = message.PublicInformationHumanizedStr;

            if (!string.IsNullOrWhiteSpace(publicInformationHumanizedStr))
            {
                sb.AppendLine("Begin Public facts");
                sb.AppendLine(publicInformationHumanizedStr);
                sb.AppendLine("End Public facts");
            }

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetChangedDistance(ChangedDistanceMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var sb = new StringBuilder();
            sb.AppendLine($"<{message.VisionFrameId}> : {message.ObjectId}");
            sb.AppendLine($"Distance: {message.Distance}");

            var publicInformationHumanizedStr = message.PublicInformationHumanizedStr;

            if (!string.IsNullOrWhiteSpace(publicInformationHumanizedStr))
            {
                sb.AppendLine("Begin Public facts");
                sb.AppendLine(publicInformationHumanizedStr);
                sb.AppendLine("End Public facts");
            }

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetDumpVisionFrame(DumpVisionFrameMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var visibleItems = message.VisibleItems;

            if (visibleItems.IsNullOrEmpty())
            {
                return logFileCreatorContext.NormalizeAsHtml($"<{message.VisionFrameId}>: Nothing is seen");
            }

            var sb = new StringBuilder();

            sb.AppendLine($"<{message.VisionFrameId}>:");

            foreach (var visibleItem in visibleItems)
            {
                sb.AppendLine($"Begin: {visibleItem.ObjectId}");

                var publicInformationHumanizedStr = visibleItem.PublicInformationHumanizedStr;

                if (!string.IsNullOrWhiteSpace(publicInformationHumanizedStr))
                {
                    sb.AppendLine("Begin Public facts");
                    sb.AppendLine(publicInformationHumanizedStr);
                    sb.AppendLine("End Public facts");
                }
                sb.AppendLine("End");
            }

            return logFileCreatorContext.NormalizeAsHtml(sb.ToString());
        }

        private string GetEndVisionFrame(EndVisionFrameMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml($"<{message.VisionFrameId}>");
        }

        private string GetOutput(OutputMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.NormalizeAsHtml(message.Message);
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

            return $"{logFileCreatorContext.NormalizeAsHtml(GetMonitoredHumanizedLabel(message.Query))}: {logFileCreatorContext.CreateImgLink(fileName.AbsoluteName, fileName.RelativeName)}";
        }

        private string GetTrace(TraceMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(logFileCreatorContext.NormalizeAsHtml(message.Message));
        }

        private string GetDebug(DebugMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(logFileCreatorContext.NormalizeAsHtml(message.Message));
        }

        private string GetInfoMessage(InfoMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(logFileCreatorContext.NormalizeAsHtml(message.Message));
        }

        private string GetWarn(WarnMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(logFileCreatorContext.NormalizeAsHtml(message.Message));
        }

        private string GetError(ErrorMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(logFileCreatorContext.NormalizeAsHtml(message.Message));
        }

        private string GetFatal(FatalMessage message, ILogFileCreatorContext logFileCreatorContext)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return logFileCreatorContext.ResolveMessagesRefs(logFileCreatorContext.NormalizeAsHtml(message.Message));
        }
    }
}
