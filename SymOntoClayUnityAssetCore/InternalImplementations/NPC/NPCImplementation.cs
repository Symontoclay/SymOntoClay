﻿using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.NPC
{
    /// <inheritdoc/>
    public class NPCImplementation: INPC
    {
        public NPCImplementation(NPCSettings settings, WorldContext context)
        {
        }

        /// <inheritdoc/>
        public bool IsDisposed => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
