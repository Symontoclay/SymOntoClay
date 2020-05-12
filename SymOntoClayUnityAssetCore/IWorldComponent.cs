using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public interface IWorldComponent : ISymOntoClayDisposable
    {
        IEntityLogger Logger { get; }
    }
}
