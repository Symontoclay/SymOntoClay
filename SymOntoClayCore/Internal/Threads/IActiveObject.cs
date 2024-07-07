using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Threads
{
    public interface IActiveObject : IDisposable
    {
        bool IsActive { get; }
        bool IsWaited { get; }
        IThreadTask TaskValue { get; }
        IThreadTask Start();
    }
}
