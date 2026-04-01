using SymOntoClay.Common.Disposing;

namespace SymOntoClay.Monitor.Internal.FileWriter
{
    public interface IMonitorNodeFileWriter: IFileWriter, ISymOntoClayDisposable
    {
        IThreadLoggerFileWriter CreateThreadLoggerFileWriter(string theadId);
    }
}
