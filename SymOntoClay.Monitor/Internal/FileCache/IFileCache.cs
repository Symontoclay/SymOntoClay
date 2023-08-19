using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.Internal.FileCache
{
    public interface IFileCache
    {
        void WriteFile(string fileName, string messageText);
        string DirectoryName { get; }
    }
}
