using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public static class MessageContentToTextConverter
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        public static string GetText(BaseMessage message)
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

        private static string GetCreateMotitorNode(CreateMotitorNodeMessage message)
        {
            return $"MotitorNode '{message.NodeId}' has been created";
        }

        private static string GetCreateThreadLogger(CreateThreadLoggerMessage message)
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

        private static string GetCallMethod(CallMethodMessage message)
        {
            var sb = new StringBuilder($"<{message.CallMethodId}> [{(message.IsSynk ? "sync" : "async")}]");

            if(string.IsNullOrWhiteSpace(message.AltMethodName))
            {
                var methodLabel = message.MethodLabel;

                if(!string.IsNullOrWhiteSpace(methodLabel.KindOfCodeItemDescriptor))
                {
                    throw new NotImplementedException();
                }

                sb.Append($" {methodLabel.Label}");

                if (methodLabel.Signatures?.Any() ?? false)
                {
#if DEBUG
                    _globalLogger.Info($"message = {message}");
#endif

                    throw new NotImplementedException();
                }

                if (methodLabel.Values?.Any() ?? false)
                {
#if DEBUG
                    _globalLogger.Info($"message = {message}");
#endif

                    throw new NotImplementedException();
                }
            }
            else
            {
                sb.Append($" {message.AltMethodName}");
            }

            return sb.ToString();
        }

        private static string GetParameter(ParameterMessage message)
        {
            fix me

            var tmpResult = $"Parameter of <{message.CallMethodId}>: '{message.Label}' = {message.HumanizedString}";//{ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(message.Base64Content, message.TypeName)}

#if DEBUG
            //_globalLogger.Info($"tmpResult = {tmpResult}");
#endif

            return tmpResult;
        }

        private static string GetEndCallMethod(EndCallMethodMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetMethodResolving(MethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetEndMethodResolving(EndMethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetActionResolving(ActionResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetEndActionResolving(EndActionResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetHostMethodResolving(HostMethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetEndHostMethodResolving(EndHostMethodResolvingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetHostMethodActivation(HostMethodActivationMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetEndHostMethodActivation(EndHostMethodActivationMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetHostMethodStarting(HostMethodStartingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetEndHostMethodStarting(EndHostMethodStartingMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetHostMethodExecution(HostMethodExecutionMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetEndHostMethodExecution(EndHostMethodExecutionMessage message)
        {
            return $"<{message.CallMethodId}>";
        }

        private static string GetSystemExpr(SystemExprMessage message)
        {
            fix me

            var tmpResult = $"Expression of <{message.CallMethodId}>: '{message.Label}' = {message.HumanizedString}";//{ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(message.Base64Content, message.TypeName)}

#if DEBUG
            //_globalLogger.Info($"tmpResult = {tmpResult}");
#endif

            return tmpResult;
        }

        private static string GetCodeFrame(CodeFrameMessage message)
        {
            return $"\n{message.HumanizedStr}\n";
        }

        private static string GetLeaveThreadExecutor(LeaveThreadExecutorMessage message)
        {
            return string.Empty;
        }

        public static string GetGoBackToPrevCodeFrame(GoBackToPrevCodeFrameMessage message)
        {
            return message.TargetActionExecutionStatusStr;
        }

        private static string GetOutput(OutputMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private static string GetTrace(TraceMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private static string GetDebug(DebugMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private static string GetInfoMessage(InfoMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private static string GetWarn(WarnMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private static string GetError(ErrorMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }

        private static string GetFatal(FatalMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }
    }
}
