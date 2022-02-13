/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
        /// Adds a game object into manual controlled area of the NPC.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="device">Describes biped device which will be using the game object.</param>
        void AddToManualControl(IGameObject obj, DeviceOfBiped device);

        /// <summary>
        /// Adds a game object into manual controlled area of the NPC.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        /// <param name="devices">Describes list of biped devices which will be using the game object.</param>
        void AddToManualControl(IGameObject obj, IList<DeviceOfBiped> devices);

        /// <summary>
        /// Removes a game object from manual controlled area of an NPC.
        /// </summary>
        /// <param name="obj">Instance of the game object.</param>
        void RemoveFromManualControl(IGameObject obj);

        /// <summary>
        /// Gets list of manual controlled game objects by the NPC.
        /// </summary>
        /// <returns>List of manual controlled game objects.</returns>
        IList<IHumanoidManualControlledObject> GetManualControlledObjects();
    }
}
