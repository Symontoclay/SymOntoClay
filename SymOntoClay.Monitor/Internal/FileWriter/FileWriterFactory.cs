using SymOntoClay.CoreHelper.SerializerAdapters;
using SymOntoClay.Monitor.Internal.FileWriter.General;
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
                    return new MonitorFileWriter(messagesDir, sessionName);
            }

            throw new NotImplementedException($"62A708F8-3DED-40DD-8D89-664328E49F80: {kindOfSerialization}");
        }
    }
}
