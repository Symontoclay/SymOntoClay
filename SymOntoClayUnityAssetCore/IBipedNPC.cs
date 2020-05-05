using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represents biped NPC (Non-Player Character)
    /// </summary>
    public interface IBipedNPC: IManualControlling, IWorldComponent
    {
        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for the NPC.
        /// </summary>
        bool EnableLogging { get; set; }
    }
}
