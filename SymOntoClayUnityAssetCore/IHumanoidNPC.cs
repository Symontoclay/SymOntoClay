/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represents biped NPC (Non-Player Character)
    /// </summary>
    public interface IHumanoidNPC: IManualControlling, IWorldComponent
    {
        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for the NPC.
        /// </summary>
        bool EnableLogging { get; set; }

        /// <summary>
        /// Performs death of the NPC.
        /// All active processes will have been stopped.
        /// Another NPCs will percept the NPC as died.
        /// </summary>
        void Die();
        string InsertFact(IMonitorLogger logger, string text);
        void RemoveFact(IMonitorLogger logger, string id);

        /// <summary>
        /// Returns storage that represents a backpack.
        /// </summary>
        IStorage BackpackStorage { get; }

        /// <summary>
        /// Adds a game object into backpack.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="obj">Instance of the game object.</param>
        void AddToBackpack(IMonitorLogger logger, IGameObject obj);

        /// <summary>
        /// Removes game object from backpack.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="obj">Instance of the game object.</param>
        void RemoveFromBackpack(IMonitorLogger logger, IGameObject obj);

        /// <summary>
        /// Gets engine context. Onkly for debugging and testing!
        /// </summary>
        IEngineContext EngineContext { get; }
    }
}
