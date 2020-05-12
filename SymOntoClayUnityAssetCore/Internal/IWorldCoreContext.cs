using SymOntoClay.Core;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal
{
    public interface IWorldCoreContext
    {
        void AddWorldComponent(IWorldCoreComponent component);
        IEntityLogger Logger { get; }
        SymOntoClay.Core.IEntityDictionary SharedDictionary { get; }
        IModulesStorage ModulesStorage { get; }
    }
}
