namespace SymOntoClay.Monitor.Internal.FileWriter
{
    public interface IMonitorFileWriter: IFileWriter, IFileWriterMetadata
    {
        IMonitorNodeFileWriter CreateMonitorNodeFileWriter(string nodeId);
    }
}
