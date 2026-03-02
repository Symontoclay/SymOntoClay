using SymOntoClay.CoreHelper.SerializerAdapters;
using System;

namespace SymOntoClay.Monitor.Internal.FileWriter
{
    public static class FileWriterFactory
    {
        public static IMonitorFileWriter CreateMonitorFileWriter(KindOfSerialization kindOfSerialization, string messagesDir, string sessionName)
        {
            switch(kindOfSerialization)
            {
                case KindOfSerialization.Json:
                    return new SymOntoClay.Monitor.Internal.FileWriter.General.MonitorFileWriter(messagesDir, sessionName);

                case KindOfSerialization.MessagePack:
                    return new SymOntoClay.Monitor.Internal.FileWriter.Binary.MonitorFileWriter(messagesDir, sessionName);
            }

            throw new NotImplementedException($"62A708F8-3DED-40DD-8D89-664328E49F80: {kindOfSerialization}");
        }
    }
}
