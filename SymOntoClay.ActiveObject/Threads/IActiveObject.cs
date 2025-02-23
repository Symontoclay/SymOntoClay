using System;

namespace SymOntoClay.ActiveObject.Threads
{
    public interface IActiveObject : IDisposable
    {
        bool IsActive { get; }
        bool IsWaited { get; }
    }
}
