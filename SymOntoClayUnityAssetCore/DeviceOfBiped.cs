/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
