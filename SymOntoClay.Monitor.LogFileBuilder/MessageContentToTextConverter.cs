using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

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

                case KindOfMessage.Info:
                    return GetInfoMessageText(message as InfoMessage);

                default:
                    //throw new ArgumentOutOfRangeException(nameof(kindOfMessage), kindOfMessage, null);
                    return string.Empty;
            }

            /*
        Output,
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
             */
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
            //return $"Parameter of {message.CallMethodId}: '{message.ParameterName}' = ";
            throw new NotImplementedException();
        }

        private static string GetInfoMessageText(InfoMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            return message.Message;
        }
    }
}
