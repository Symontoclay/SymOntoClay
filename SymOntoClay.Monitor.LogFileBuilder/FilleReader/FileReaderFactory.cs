using SymOntoClay.CoreHelper.SerializerAdapters;
using System;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader
{
    public static class FileReaderFactory
    {
        public static ILogFileReader CreateMonitorLogFileReader(KindOfSerialization kindOfSerialization)
        {
            switch(kindOfSerialization)
            {
                case KindOfSerialization.Json:
                    return new SymOntoClay.Monitor.LogFileBuilder.FilleReader.General.MonitorLogFileReader();

                case KindOfSerialization.MessagePack:
                    return new SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary.MonitorLogFileReader();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, "AD37B0E9-E36C-42E8-B0C1-7B7528D6AE23");
            }
        }
    }
}
