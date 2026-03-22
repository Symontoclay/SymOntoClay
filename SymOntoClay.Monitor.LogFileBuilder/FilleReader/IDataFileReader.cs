using SymOntoClay.Common.Disposing;

namespace SymOntoClay.Monitor.LogFileBuilder.FilleReader
{
    public interface IDataFileReader: ISymOntoClayDisposable
    {
        byte[] Read(string fileName, long startPosition, int dataLength);
    }
}
