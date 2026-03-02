using SymOntoClay.Common.Disposing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Internal.FileWriter
{
    public interface IMonitorNodeFileWriter: IFileWriter, ISymOntoClayDisposable
    {
        IThreadLoggerFileWriter CreateThreadLoggerFileWriter(string theadId);
    }
}
