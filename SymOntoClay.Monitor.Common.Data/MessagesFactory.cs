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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    public static class MessagesFactory
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static BaseMessage ReadMessage(string content, KindOfMessage kindOfMessage)
        {
#if DEBUG
            //_globalLogger.Info($"content = {content}");
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
#endif

            switch (kindOfMessage)
            {
                case KindOfMessage.CreateMonitorNode:
                    return JsonConvert.DeserializeObject<CreateMonitorNodeMessage>(content);

                case KindOfMessage.CreateThreadLogger:
                    return JsonConvert.DeserializeObject<CreateThreadLoggerMessage>(content);

                case KindOfMessage.AddEndpoint:
                    return JsonConvert.DeserializeObject<AddEndpointMessage>(content);

                case KindOfMessage.CallMethod:
                    return JsonConvert.DeserializeObject<CallMethodMessage>(content);

                case KindOfMessage.Parameter:
                    return JsonConvert.DeserializeObject<ParameterMessage>(content);

                case KindOfMessage.EndCallMethod:
                    return JsonConvert.DeserializeObject<EndCallMethodMessage>(content);

                case KindOfMessage.MethodResolving:
                    return JsonConvert.DeserializeObject<MethodResolvingMessage>(content);

                case KindOfMessage.EndMethodResolving:
                    return JsonConvert.DeserializeObject<EndMethodResolvingMessage>(content);

                case KindOfMessage.ActionResolving:
                    return JsonConvert.DeserializeObject<ActionResolvingMessage>(content);

                case KindOfMessage.EndActionResolving:
                    return JsonConvert.DeserializeObject<EndActionResolvingMessage>(content);

                case KindOfMessage.HostMethodResolving:
                    return JsonConvert.DeserializeObject<HostMethodResolvingMessage>(content);

                case KindOfMessage.EndHostMethodResolving:
                    return JsonConvert.DeserializeObject<EndHostMethodResolvingMessage>(content);

                case KindOfMessage.HostMethodActivation:
                    return JsonConvert.DeserializeObject<HostMethodActivationMessage>(content);

                case KindOfMessage.EndHostMethodActivation:
                    return JsonConvert.DeserializeObject<EndHostMethodActivationMessage>(content);

                case KindOfMessage.HostMethodStarting:
                    return JsonConvert.DeserializeObject<HostMethodStartingMessage>(content);

                case KindOfMessage.EndHostMethodStarting:
                    return JsonConvert.DeserializeObject<EndHostMethodStartingMessage>(content);

                case KindOfMessage.HostMethodExecution:
                    return JsonConvert.DeserializeObject<HostMethodExecutionMessage>(content);

                case KindOfMessage.EndHostMethodExecution:
                    return JsonConvert.DeserializeObject<EndHostMethodExecutionMessage>(content);

                case KindOfMessage.SystemExpr:
                    return JsonConvert.DeserializeObject<SystemExprMessage>(content);

                case KindOfMessage.CodeFrame:
                    return JsonConvert.DeserializeObject<CodeFrameMessage>(content);

                case KindOfMessage.LeaveThreadExecutor:
                    return JsonConvert.DeserializeObject<LeaveThreadExecutorMessage>(content);

                case KindOfMessage.GoBackToPrevCodeFrame:
                    return JsonConvert.DeserializeObject<GoBackToPrevCodeFrameMessage>(content);

                case KindOfMessage.StartProcessInfo:
                    return JsonConvert.DeserializeObject<StartProcessInfoMessage>(content);

                case KindOfMessage.CancelProcessInfo:
                    return JsonConvert.DeserializeObject<CancelProcessInfoMessage>(content);

                case KindOfMessage.WeakCancelProcessInfo:
                    return JsonConvert.DeserializeObject<WeakCancelProcessInfoMessage>(content);

                case KindOfMessage.CancelInstanceExecution:
                    return JsonConvert.DeserializeObject<CancelInstanceExecutionMessage>(content);

                case KindOfMessage.SetExecutionCoordinatorStatus:
                    return JsonConvert.DeserializeObject<SetExecutionCoordinatorStatusMessage>(content);

                case KindOfMessage.SetProcessInfoStatus:
                    return JsonConvert.DeserializeObject<SetProcessInfoStatusMessage>(content);

                case KindOfMessage.WaitProcessInfo:
                    return JsonConvert.DeserializeObject<WaitProcessInfoMessage>(content);

                case KindOfMessage.RunLifecycleTrigger:
                    return JsonConvert.DeserializeObject<RunLifecycleTriggerMessage>(content);

                case KindOfMessage.DoTriggerSearch:
                    return JsonConvert.DeserializeObject<DoTriggerSearchMessage>(content);

                case KindOfMessage.EndDoTriggerSearch:
                    return JsonConvert.DeserializeObject<EndDoTriggerSearchMessage>(content);

                case KindOfMessage.SetConditionalTrigger:
                    return JsonConvert.DeserializeObject<SetConditionalTriggerMessage>(content);

                case KindOfMessage.ResetConditionalTrigger:
                    return JsonConvert.DeserializeObject<ResetConditionalTriggerMessage>(content);

                case KindOfMessage.RunSetExprOfConditionalTrigger:
                    return JsonConvert.DeserializeObject<RunSetExprOfConditionalTriggerMessage>(content);

                case KindOfMessage.EndRunSetExprOfConditionalTrigger:
                    return JsonConvert.DeserializeObject<EndRunSetExprOfConditionalTriggerMessage>(content);

                case KindOfMessage.RunResetExprOfConditionalTrigger:
                    return JsonConvert.DeserializeObject<RunResetExprOfConditionalTriggerMessage>(content);

                case KindOfMessage.EndRunResetExprOfConditionalTrigger:
                    return JsonConvert.DeserializeObject<EndRunResetExprOfConditionalTriggerMessage>(content);

                case KindOfMessage.ActivateIdleAction:
                    return JsonConvert.DeserializeObject<ActivateIdleActionMessage>(content);

                case KindOfMessage.LogicalSearchExplain:
                    return JsonConvert.DeserializeObject<LogicalSearchExplainMessage>(content);

                case KindOfMessage.AddFactOrRuleTriggerResult:
                    return JsonConvert.DeserializeObject<AddFactOrRuleTriggerResultMessage>(content);

                case KindOfMessage.AddFactToLogicalStorage:
                    return JsonConvert.DeserializeObject<AddFactToLogicalStorageMessage>(content);

                case KindOfMessage.RemoveFactFromLogicalStorage:
                    return JsonConvert.DeserializeObject<RemoveFactFromLogicalStorageMessage>(content);

                case KindOfMessage.RefreshLifeTimeInLogicalStorage:
                    return JsonConvert.DeserializeObject<RefreshLifeTimeInLogicalStorageMessage>(content);

                case KindOfMessage.PutFactForRemovingFromLogicalStorage:
                    return JsonConvert.DeserializeObject<PutFactForRemovingFromLogicalStorageMessage>(content);

                case KindOfMessage.StartThreadTask:
                    return JsonConvert.DeserializeObject<StartTaskMessage>(content);

                case KindOfMessage.StopThreadTask:
                    return JsonConvert.DeserializeObject<StopTaskMessage>(content);

                case KindOfMessage.StartBuildPlan:
                    return JsonConvert.DeserializeObject<StartBuildPlanMessage>(content);

                case KindOfMessage.StopBuildPlan:
                    return JsonConvert.DeserializeObject<StopBuildPlanMessage>(content);

                case KindOfMessage.BeginVisionFrame:
                    return JsonConvert.DeserializeObject<BeginVisionFrameMessage>(content);

                case KindOfMessage.BecomeInvisible:
                    return JsonConvert.DeserializeObject<BecomeInvisibleMessage>(content);

                case KindOfMessage.BecomeVisible:
                    return JsonConvert.DeserializeObject<BecomeVisibleMessage>(content);

                case KindOfMessage.ChangedAddFocus:
                    return JsonConvert.DeserializeObject<ChangedAddFocusMessage>(content);

                case KindOfMessage.ChangedRemoveFocus:
                    return JsonConvert.DeserializeObject<ChangedRemoveFocusMessage>(content);

                case KindOfMessage.ChangedDistance:
                    return JsonConvert.DeserializeObject<ChangedDistanceMessage>(content);

                case KindOfMessage.DumpVisionFrame:
                    return JsonConvert.DeserializeObject<DumpVisionFrameMessage>(content);

                case KindOfMessage.EndVisionFrame:
                    return JsonConvert.DeserializeObject<EndVisionFrameMessage>(content);

                case KindOfMessage.Output:
                    return JsonConvert.DeserializeObject<OutputMessage>(content);

                case KindOfMessage.Trace:
                    return JsonConvert.DeserializeObject<TraceMessage>(content);

                case KindOfMessage.Debug:
                    return JsonConvert.DeserializeObject<DebugMessage>(content);

                case KindOfMessage.Info:
                    return JsonConvert.DeserializeObject<InfoMessage>(content);

                case KindOfMessage.Warn:
                    return JsonConvert.DeserializeObject<WarnMessage>(content);

                case KindOfMessage.Error:
                    return JsonConvert.DeserializeObject<ErrorMessage>(content);

                case KindOfMessage.Fatal:
                    return JsonConvert.DeserializeObject<FatalMessage>(content);
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfMessage), kindOfMessage, null);
            }
        }
    }
}
