using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.ActiveObject.Threads
{
    public interface IActiveAsyncObject: IActiveObject
    {
        IThreadTask TaskValue { get; }
        IThreadTask Start();
    }
}
