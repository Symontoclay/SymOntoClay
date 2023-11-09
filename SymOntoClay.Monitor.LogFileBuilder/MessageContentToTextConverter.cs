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
        //private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
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
                    return Get(message as Message);

                case KindOfMessage.EndMethodResolving:
                    return Get(message as Message);

                case KindOfMessage.ActionResolving:
                    return Get(message as Message);

                case KindOfMessage.EndActionResolving:
                    return Get(message as Message);

                case KindOfMessage.HostMethodResolving:
                    return Get(message as Message);

                case KindOfMessage.EndHostMethodResolving:
                    return Get(message as Message);

                case KindOfMessage.HostMethodActivation:
                    return Get(message as Message);

                case KindOfMessage.EndHostMethodActivation:
                    return Get(message as Message);

                case KindOfMessage.HostMethodExecution:
                    return Get(message as Message);

                case KindOfMessage.EndHostMethodExecution:
                    return Get(message as Message);

                case KindOfMessage.SystemExpr:
                    return Get(message as Message);

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

            if(!string.IsNullOrWhiteSpace(message.ParentThreadId))
            {
                sb.Append($" with ParentThreadId '{message.ParentThreadId}'");
            }

            return sb.ToString();
        }

        private static string GetCallMethod(CallMethodMessage message)
        {
            return $"<{message.CallMethodId}> {message.MethodName}";
        }

        private static string GetParameter(ParameterMessage message)
        {
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
