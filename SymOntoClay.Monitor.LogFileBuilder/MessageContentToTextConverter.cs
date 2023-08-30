using SymOntoClay.Monitor.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class MessageContentToTextConverter
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static string GetText(BaseMessage message)
        {
#if DEBUG
            //_globalLogger.Info($"message = {message}");
#endif

            var kindOfMessage = message.KindOfMessage;

            switch (kindOfMessage)
            {
                case KindOfMessage.Info:
                    return GetInfoMessageText(message as InfoMessage);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfMessage), kindOfMessage, null);
            }
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
