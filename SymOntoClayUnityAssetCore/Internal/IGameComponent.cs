using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IGameComponent: ISymOntoClayDisposable
    {
        void LoadFromSourceCode();
        void BeginStarting();
        bool IsWaited { get; }
    }
}
