using SymOntoClay.Monitor.Common.Data;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SymOntoClay.CoreHelper.CollectionsHelpers;

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

        public string GetText(BaseMessage message)
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

                case KindOfMessage.Output:
                    return GetOutput(message as OutputMessage);

                case KindOfMessage.Trace:
                    return GetTrace(message as TraceMessage);

                case KindOfMessage.Debug:
                    return GetDebug(message as DebugMessage);

                case KindOfMessage.Info:
                    return GetInfoMessage(message as InfoMessage);

                case KindOfMessage.Warn:
                    return GetWarn(message as WarnMessage);

                case KindOfMessage.Error:
                    return GetError(message as ErrorMessage);

                case KindOfMessage.Fatal:
                    return GetFatal(message as FatalMessage);

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
            _globalLogger.Info($"argument = {argument}");
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
            _globalLogger.Info($"sb = {sb}");
#endif

            return sb.ToString();
        }

        private string GetMonitoredHumanizedMethodParameterValue(MonitoredHumanizedMethodParameterValue value)
        {
#if DEBUG
            _globalLogger.Info($"value = {value}");
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
            _globalLogger.Info($"label = {label}");
#endif

            var sb = new StringBuilder();

            var kindOfCodeItemDescriptor = label.KindOfCodeItemDescriptor;

            if (!string.IsNullOrWhiteSpace(kindOfCodeItemDescriptor))
            {
                if(kindOfCodeItemDescriptor != "fun")
                {
#if DEBUG
                    _globalLogger.Info($"label = {label}");
#endif

                    throw new NotImplementedException();
                }
            }

            sb.Append($" {label.Label}");

            if(_options.EnableMethodSignatureArguments)
            {
                var signatures = label.Signatures;

                if (signatures != null)
                {
#if DEBUG
                    _globalLogger.Info($"label = {label}");
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
                    _globalLogger.Info($"label = {label}");
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

            var chainOfProcessInfo = message.ChainOfProcessInfo;

            if (!chainOfProcessInfo.IsNullOrEmpty())
            {
#if DEBUG
                _globalLogger.Info($"message = {message}");
#endif

                foreach(var item in chainOfProcessInfo)
                {
                    labelsStrList.Add(GetMonitoredHumanizedLabel(item));
                }
            }

            labelsStrList.Add(GetMonitoredHumanizedLabel(message.MethodLabel, message.AltMethodName));

            return $"<{message.CallMethodId}> [{(message.IsSynk ? "sync" : "async")}] {string.Join(" -> ", labelsStrList)}";
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
            return $"\n{message.HumanizedStr}\n";
        }

        private string GetLeaveThreadExecutor(LeaveThreadExecutorMessage message)
        {
            return string.Empty;
        }

        public static string GetGoBackToPrevCodeFrame(GoBackToPrevCodeFrameMessage message)
        {
            return message.TargetActionExecutionStatusStr;
        }

        private string GetOutput(OutputMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private string GetTrace(TraceMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private string GetDebug(DebugMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private string GetInfoMessage(InfoMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private string GetWarn(WarnMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private string GetError(ErrorMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private string GetFatal(FatalMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }
    }
}
