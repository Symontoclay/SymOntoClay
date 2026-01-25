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
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.SerializerAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Common.Data
{
    public class MessagesFactory
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public MessagesFactory(KindOfSerialization kindOfSerialization)
        {
#if DEBUG
            _globalLogger.Info($"kindOfSerialization = {kindOfSerialization}");
#endif

            _kindOfSerialization = kindOfSerialization;

            _serializerAdapter = SerializerAdapterFactory.Create(kindOfSerialization);
        }

        private readonly KindOfSerialization _kindOfSerialization;
        private readonly ISerializerAdapter _serializerAdapter;

        public BaseMessage ReadMessage(byte[] data, KindOfMessage kindOfMessage)
        {
#if DEBUG
            //_globalLogger.Info($"content = {content}");
            //_globalLogger.Info($"kindOfMessage = {kindOfMessage}");
#endif

            if(_kindOfSerialization == KindOfSerialization.MessagePack)
            {
                return _serializerAdapter.Deserialize<BaseMessage>(data);
            }

            switch (kindOfMessage)
            {
                case KindOfMessage.CreateMonitorNode:
                    return _serializerAdapter.Deserialize<CreateMonitorNodeMessage>(data);//Y

                case KindOfMessage.CreateThreadLogger:
                    return _serializerAdapter.Deserialize<CreateThreadLoggerMessage>(data);//Y

                case KindOfMessage.AddEndpoint:
                    return _serializerAdapter.Deserialize<AddEndpointMessage>(data);//Y

                case KindOfMessage.CallMethod:
                    return _serializerAdapter.Deserialize<CallMethodMessage>(data);//Y

                case KindOfMessage.Parameter:
                    return _serializerAdapter.Deserialize<ParameterMessage>(data);//Y

                case KindOfMessage.EndCallMethod:
                    return _serializerAdapter.Deserialize<EndCallMethodMessage>(data);//Y

                case KindOfMessage.MethodResolving:
                    return _serializerAdapter.Deserialize<MethodResolvingMessage>(data);//Y

                case KindOfMessage.EndMethodResolving:
                    return _serializerAdapter.Deserialize<EndMethodResolvingMessage>(data);//Y

                case KindOfMessage.ActionResolving:
                    return _serializerAdapter.Deserialize<ActionResolvingMessage>(data);//Y

                case KindOfMessage.EndActionResolving:
                    return _serializerAdapter.Deserialize<EndActionResolvingMessage>(data);//Y

                case KindOfMessage.HostMethodResolving:
                    return _serializerAdapter.Deserialize<HostMethodResolvingMessage>(data);//Y

                case KindOfMessage.EndHostMethodResolving:
                    return _serializerAdapter.Deserialize<EndHostMethodResolvingMessage>(data);//Y

                case KindOfMessage.HostMethodActivation:
                    return _serializerAdapter.Deserialize<HostMethodActivationMessage>(data);//Y

                case KindOfMessage.EndHostMethodActivation:
                    return _serializerAdapter.Deserialize<EndHostMethodActivationMessage>(data);//Y

                case KindOfMessage.HostMethodStarting:
                    return _serializerAdapter.Deserialize<HostMethodStartingMessage>(data);//Y

                case KindOfMessage.EndHostMethodStarting:
                    return _serializerAdapter.Deserialize<EndHostMethodStartingMessage>(data);//Y

                case KindOfMessage.HostMethodExecution:
                    return _serializerAdapter.Deserialize<HostMethodExecutionMessage>(data);//Y

                case KindOfMessage.EndHostMethodExecution:
                    return _serializerAdapter.Deserialize<EndHostMethodExecutionMessage>(data);//Y

                case KindOfMessage.SystemExpr:
                    return _serializerAdapter.Deserialize<SystemExprMessage>(data);//Y

                case KindOfMessage.CodeFrame:
                    return _serializerAdapter.Deserialize<CodeFrameMessage>(data);//Y

                case KindOfMessage.LeaveThreadExecutor:
                    return _serializerAdapter.Deserialize<LeaveThreadExecutorMessage>(data);//Y

                case KindOfMessage.GoBackToPrevCodeFrame:
                    return _serializerAdapter.Deserialize<GoBackToPrevCodeFrameMessage>(data);//Y

                case KindOfMessage.StartProcessInfo:
                    return _serializerAdapter.Deserialize<StartProcessInfoMessage>(data);//Y

                case KindOfMessage.CancelProcessInfo:
                    return _serializerAdapter.Deserialize<CancelProcessInfoMessage>(data);//Y

                case KindOfMessage.WeakCancelProcessInfo:
                    return _serializerAdapter.Deserialize<WeakCancelProcessInfoMessage>(data);//Y

                case KindOfMessage.CancelInstanceExecution:
                    return _serializerAdapter.Deserialize<CancelInstanceExecutionMessage>(data);//Y

                case KindOfMessage.SetExecutionCoordinatorStatus:
                    return _serializerAdapter.Deserialize<SetExecutionCoordinatorStatusMessage>(data);//Y

                case KindOfMessage.SetProcessInfoStatus:
                    return _serializerAdapter.Deserialize<SetProcessInfoStatusMessage>(data);//Y

                case KindOfMessage.WaitProcessInfo:
                    return _serializerAdapter.Deserialize<WaitProcessInfoMessage>(data);//Y

                case KindOfMessage.RunLifecycleTrigger:
                    return _serializerAdapter.Deserialize<RunLifecycleTriggerMessage>(data);//Y

                case KindOfMessage.DoTriggerSearch:
                    return _serializerAdapter.Deserialize<DoTriggerSearchMessage>(data);//Y

                case KindOfMessage.EndDoTriggerSearch:
                    return _serializerAdapter.Deserialize<EndDoTriggerSearchMessage>(data);//Y

                case KindOfMessage.SetConditionalTrigger:
                    return _serializerAdapter.Deserialize<SetConditionalTriggerMessage>(data);//<=

                case KindOfMessage.ResetConditionalTrigger:
                    return _serializerAdapter.Deserialize<ResetConditionalTriggerMessage>(data);

                case KindOfMessage.RunSetExprOfConditionalTrigger:
                    return _serializerAdapter.Deserialize<RunSetExprOfConditionalTriggerMessage>(data);

                case KindOfMessage.EndRunSetExprOfConditionalTrigger:
                    return _serializerAdapter.Deserialize<EndRunSetExprOfConditionalTriggerMessage>(data);

                case KindOfMessage.RunResetExprOfConditionalTrigger:
                    return _serializerAdapter.Deserialize<RunResetExprOfConditionalTriggerMessage>(data);

                case KindOfMessage.EndRunResetExprOfConditionalTrigger:
                    return _serializerAdapter.Deserialize<EndRunResetExprOfConditionalTriggerMessage>(data);

                case KindOfMessage.ActivateIdleAction:
                    return _serializerAdapter.Deserialize<ActivateIdleActionMessage>(data);

                case KindOfMessage.LogicalSearchExplain:
                    return _serializerAdapter.Deserialize<LogicalSearchExplainMessage>(data);

                case KindOfMessage.AddFactOrRuleTriggerResult:
                    return _serializerAdapter.Deserialize<AddFactOrRuleTriggerResultMessage>(data);

                case KindOfMessage.AddFactToLogicalStorage:
                    return _serializerAdapter.Deserialize<AddFactToLogicalStorageMessage>(data);

                case KindOfMessage.RemoveFactFromLogicalStorage:
                    return _serializerAdapter.Deserialize<RemoveFactFromLogicalStorageMessage>(data);

                case KindOfMessage.RefreshLifeTimeInLogicalStorage:
                    return _serializerAdapter.Deserialize<RefreshLifeTimeInLogicalStorageMessage>(data);

                case KindOfMessage.PutFactForRemovingFromLogicalStorage:
                    return _serializerAdapter.Deserialize<PutFactForRemovingFromLogicalStorageMessage>(data);

                case KindOfMessage.StartThreadTask:
                    return _serializerAdapter.Deserialize<StartTaskMessage>(data);

                case KindOfMessage.StopThreadTask:
                    return _serializerAdapter.Deserialize<StopTaskMessage>(data);

                case KindOfMessage.StartBuildPlan:
                    return _serializerAdapter.Deserialize<StartBuildPlanMessage>(data);

                case KindOfMessage.StopBuildPlan:
                    return _serializerAdapter.Deserialize<StopBuildPlanMessage>(data);

                case KindOfMessage.BeginVisionFrame:
                    return _serializerAdapter.Deserialize<BeginVisionFrameMessage>(data);

                case KindOfMessage.BecomeInvisible:
                    return _serializerAdapter.Deserialize<BecomeInvisibleMessage>(data);

                case KindOfMessage.BecomeVisible:
                    return _serializerAdapter.Deserialize<BecomeVisibleMessage>(data);

                case KindOfMessage.ChangedAddFocus:
                    return _serializerAdapter.Deserialize<ChangedAddFocusMessage>(data);

                case KindOfMessage.ChangedRemoveFocus:
                    return _serializerAdapter.Deserialize<ChangedRemoveFocusMessage>(data);

                case KindOfMessage.ChangedDistance:
                    return _serializerAdapter.Deserialize<ChangedDistanceMessage>(data);

                case KindOfMessage.DumpVisionFrame:
                    return _serializerAdapter.Deserialize<DumpVisionFrameMessage>(data);

                case KindOfMessage.EndVisionFrame:
                    return _serializerAdapter.Deserialize<EndVisionFrameMessage>(data);

                case KindOfMessage.Output:
                    return _serializerAdapter.Deserialize<OutputMessage>(data);

                case KindOfMessage.Trace:
                    return _serializerAdapter.Deserialize<TraceMessage>(data);

                case KindOfMessage.Debug:
                    return _serializerAdapter.Deserialize<DebugMessage>(data);

                case KindOfMessage.Info:
                    return _serializerAdapter.Deserialize<InfoMessage>(data);

                case KindOfMessage.Warn:
                    return _serializerAdapter.Deserialize<WarnMessage>(data);

                case KindOfMessage.Error:
                    return _serializerAdapter.Deserialize<ErrorMessage>(data);

                case KindOfMessage.Fatal:
                    return _serializerAdapter.Deserialize<FatalMessage>(data);
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfMessage), kindOfMessage, null);
            }
        }
    }
}
