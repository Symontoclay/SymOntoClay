using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represents part of body of the NPC or Player which are required for doing actions or using things (game objects). 
    /// </summary>
    public enum DeviceOfBiped
    {
        /// <summary>
        /// Represents the head of the NPC or Player.
        /// </summary>
        Head,

        /// <summary>
        /// Represents the right hand  of the NPC or Player.
        /// </summary>
        RightHand,

        /// <summary>
        /// Represents the left hand  of the NPC or Player.
        /// </summary>
        LeftHand,

        /// <summary>
        /// Represents the right  of the NPC or Player.
        /// </summary>
        RightLeg,

        /// <summary>
        /// Represents the left leg  of the NPC or Player.
        /// </summary>
        LeftLeg
    }
}
