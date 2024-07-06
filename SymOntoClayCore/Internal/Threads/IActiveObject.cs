using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Threads
{
    public interface IActiveObject : IDisposable
    {
        bool IsActive { get; }
        bool IsWaited { get; }
        Value TaskValue { get; }
        Value Start();
    }
}
