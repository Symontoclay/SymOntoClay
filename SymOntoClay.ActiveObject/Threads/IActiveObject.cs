using SymOntoClay.ActiveObject.EventsInterfaces;
using System;

namespace SymOntoClay.ActiveObject.Threads
{
    public interface IActiveObject : IDisposable
    {
        bool IsActive { get; }
        bool IsWaited { get; }

        void AddOnCompletedHandler(IOnCompletedActiveObjectHandler handler);
        void RemoveOnCompletedHandler(IOnCompletedActiveObjectHandler handler);
    }
}
