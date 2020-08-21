using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    /// <summary>
    /// Repseresents manual controlled game object.
    /// </summary>
    public interface IInternalManualControlledObject
    {
        /// <summary>
        /// Gets  manual controlled game object.
        /// </summary>
        IGameObject GameObject { get; }

        /// <summary>
        /// Gets list of devices of the NPC or Player which are using the game object.
        /// </summary>
        IList<int> Devices { get; }
    }
}
