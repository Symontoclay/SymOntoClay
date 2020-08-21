using SymOntoClay.UnityAsset.Core;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.InternalImplementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSandbox.CoreHostListener
{
    public class TstBaseManualControllingGameComponent: BaseManualControllingGameComponent
    {
        public TstBaseManualControllingGameComponent(BaseManualControllingGameComponentSettings settings, IWorldCoreGameComponentContext worldContext)
            : base(settings, worldContext)
        {
        }
    }
}
