using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Threads
{
    public interface IActivePeriodicObjectContext : IActivePeriodicObjectCommonContext
    {
        void AddChildActiveObject(IActivePeriodicObject activeObject);
        void RemoveChildActiveObject(IActivePeriodicObject activeObject);

        void WaitWhenAllIsNotWaited();

        void StartAll();
        void StopAll();
    }
}
