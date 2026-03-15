using SymOntoClay.CoreHelper.SerializerAdapters;
using System;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader
{
    public static class FileReaderFactory
    {
        public static IFileReader CreateMonitorFileReader(KindOfSerialization kindOfSerialization)
        {
            switch(kindOfSerialization)
            {
                case KindOfSerialization.MessagePack:
                    return new SymOntoClay.Monitor.LogFileBuilder.FilleReader.Binary.MonitorFileReader();

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfSerialization), kindOfSerialization, "AD37B0E9-E36C-42E8-B0C1-7B7528D6AE23");
            }
        }
    }
}
