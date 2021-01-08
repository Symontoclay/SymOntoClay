using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public interface IDeferredInitialized
    {
        void Initialize(IWorldCoreGameComponentContext worldContext);
    }
}
