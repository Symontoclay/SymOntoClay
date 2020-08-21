using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represent aspect as manual controlling things by the NPC or Player.
    /// </summary>
    public interface IManualControlling
    {
        /// <summary>
        /// Adds a game object into manual controlled area of the NPC or Player.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="device">Describes biped device which will be using the game object.</param>
        void AddToManualControl(IGameObject obj, DeviceOfBiped device);

        /// <summary>
        /// Adds a game object into manual controlled area of the NPC or Player.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="devices">Describes list of biped devices which will be using the game object.</param>
        void AddToManualControl(IGameObject obj, IList<DeviceOfBiped> devices);

        /// <summary>
        /// Removes a game object from manual controlled area of an NPC or Player.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        void RemoveFromManualControl(IGameObject obj);

        /// <summary>
        /// Gets list of manual controlled game objects by the NPC or Player.
        /// </summary>
        /// <returns>List of manual controlled game objects.</returns>
        IList<IBipedManualControlledObject> GetManualControlledObjects();
    }
}
