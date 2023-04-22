using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IProcessInfoEventHandler : IDisposable
    {
        IProcessInfo ProcessInfo { get; set; }
        void Run();
    }
}
