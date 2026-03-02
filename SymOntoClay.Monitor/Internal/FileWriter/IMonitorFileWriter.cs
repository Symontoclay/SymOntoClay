using SymOntoClay.Common.Disposing;

namespace SymOntoClay.Monitor.Internal.FileWriter
{
    public interface IMonitorFileWriter: IFileWriter, IFileWriterMetadata, ISymOntoClayDisposable
    {
        IMonitorNodeFileWriter CreateMonitorNodeFileWriter(string nodeId);
    }
}
