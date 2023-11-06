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
