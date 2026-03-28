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
                case KindOfSerialization.MessagePack:
                    return new SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary.MonitorLogFileReader();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, "AD37B0E9-E36C-42E8-B0C1-7B7528D6AE23");
            }
        }

        public static IDataFileReader CreateMonitorDataFileReader(KindOfSerialization kindOfSerialization)
        {
            switch (kindOfSerialization)
            {
                case KindOfSerialization.MessagePack:
                    return new SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary.MonitorDataFileReader();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, "5541661A-552E-4818-810C-78EE85391078");
            }
        }
    }
}
