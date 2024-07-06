using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Threads
{
    public interface IActiveOnceObject: IActiveObject
    {
        OnceDelegate OnceMethod { get; }
    }
}
