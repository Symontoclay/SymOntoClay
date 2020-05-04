using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IGameComponent: ISymOntoClayDisposable
    {
        void BeginStarting();
        bool IsReadyForActivating { get; }
    }
}
