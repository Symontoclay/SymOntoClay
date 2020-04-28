using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.NPC
{
    public class NPCImplementation: INPC
    {
        public NPCImplementation(NPCSettings settings, WorldContext context)
        {
        }

        public bool IsDisposed => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
