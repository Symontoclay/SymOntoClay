using SymOntoClay.Monitor.Common.Data;
using System.Collections.Generic;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader
{
    public interface IFileReader
    {
        List<IndexFileRowRecord> GetIndexFileRowRecords(string targetDirectoryName, IEnumerable<KindOfMessage> targetKindOfMessages, IEnumerable<string> targetNodes, IEnumerable<string> targetThreads);
    }
}
