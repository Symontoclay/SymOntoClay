using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.Threads
{
    public interface IActivePeriodicObjectContext: IActivePeriodicObjectCommonContext
    {
        void AddChildActiveObject(IActivePeriodicObject activeObject);
        void RemoveChildActiveObject(IActivePeriodicObject activeObject);

        void WaitWhenAllIsNotWaited();

        void StartAll();
        void StopAll();
    }
}
