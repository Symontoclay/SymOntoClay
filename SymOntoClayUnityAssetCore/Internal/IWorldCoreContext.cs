using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IWorldCoreContext
    {
        void AddWorldComponent(IWorldCoreComponent component);
        ILogger Logger { get; }
    }
}
