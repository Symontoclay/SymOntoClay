using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Repseresents manual controlled game object.
    /// </summary>
    public interface IManualControlledObject
    {
        /// <summary>
        /// Gets  manual controlled game object.
        /// </summary>
        IGameObject GameObject { get; }

        /// <summary>
        /// Gets list of devices of of the NPC or Player which are using the game object.
        /// </summary>
        IList<DeviceOfBiped> Devices { get; }
    }
}
