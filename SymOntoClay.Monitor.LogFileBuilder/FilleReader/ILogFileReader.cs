using SymOntoClay.Monitor.Common.Data;
using System.Collections.Generic;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader
{
    public interface ILogFileReader
    {
        List<LogFileRowRecord> GetIndexFileRowRecords(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads);
        byte[] ReadData(string fileName, byte[] data);
    }
}
