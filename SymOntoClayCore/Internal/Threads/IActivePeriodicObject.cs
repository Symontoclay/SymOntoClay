using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Threads
{
    public interface IActivePeriodicObject: IDisposable
    {
        PeriodicDelegate PeriodicMethod { get; set; }
        bool IsActive { get; }
        bool IsWaited { get; }
        Value TaskValue { get; }
        Value Start();
        void Stop();
    }
}
