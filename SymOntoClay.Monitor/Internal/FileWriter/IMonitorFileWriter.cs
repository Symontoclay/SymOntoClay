namespace SymOntoClay.Monitor.Internal.FileWriter
{
    public interface IMonitorFileWriter: IFileWriter
    {
        IMonitorNodeFileWriter CreateMonitorNodeFileWriter(string nodeId);
    }
}
