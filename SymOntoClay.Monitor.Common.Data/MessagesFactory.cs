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
                case KindOfMessage.CreateMotitorNode:
                    return JsonConvert.DeserializeObject<CreateMotitorNodeMessage>(content);

                case KindOfMessage.CreateThreadLogger:
                    return JsonConvert.DeserializeObject<CreateThreadLoggerMessage>(content);

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
