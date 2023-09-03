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
            _globalLogger.Info($"message = {message}");
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
            throw new NotImplementedException();
        }

        private static string GetCreateThreadLogger(CreateThreadLoggerMessage message)
        {
            return $"MotitorNode '{message.NodeId}' has created ThreadLogger '{message.ThreadId}'";
        }

        private static string GetCallMethod(CallMethodMessage message)
        {
            return message.MethodName;
        }

        private static string GetParameter(ParameterMessage message)
        {
            var tmpResult = $"Parameter of {message.CallMethodId}: '{message.ParameterName}' = {ObjectToHumanizedStringConverter.FromBase64StringToHumanizedString(message.Base64Content, message.TypeName)}";

#if DEBUG
            _globalLogger.Info($"tmpResult = {tmpResult}");
#endif

            return tmpResult;
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
