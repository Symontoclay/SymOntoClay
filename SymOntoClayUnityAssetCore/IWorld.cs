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

using SymOntoClay.CoreHelper;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represents core of a game world.
    /// </summary>
    public interface IWorld: IWorldComponent
    {
        /// <summary>
        /// Sets general settings into the instance.
        /// This method can be run just once in the start of game or application.
        /// You can not use the framework before calling this method.
        /// Calling other methods before this will provoke exceptions.
        /// You can not call the method after disposing the instance.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        void SetSettings(WorldSettings settings);

        /// <summary>
        /// Adds platform type convertor manually.
        /// You should add platform type convertor manually using Unity3d, bacause automatic loading convertors provoces exception on Unity3d.
        /// </summary>
        /// <param name="convertor">An instance of platform type convertor.</param>
        void AddConvertor(IPlatformTypesConverter convertor);

        /// <summary>
        /// Gets or sets value of enable logging.
        /// It allows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets value of enable remote connection.
        /// It allows enable or disable remote connection for whole components synchronously.
        /// It doesn't touch local logging.
        /// </summary>
        bool EnableRemoteConnection { get; set; }

        /// <summary>
        /// Registers an instance of NPC by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        IHumanoidNPC GetHumanoidNPC(HumanoidNPCSettings settings);

        /// <summary>
        /// Registers an instance of Player by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        IPlayer GetPlayer(PlayerSettings settings);

        /// <summary>
        /// Registers an instance of game object by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        IGameObject GetGameObject(GameObjectSettings settings);

        /// <summary>
        /// Registers an instance of place by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        IPlace GetPlace(PlaceSettings settings);

        /// <summary>
        /// Starts execution loaded image.
        /// If there was not loading image default image will be created from source code.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops execution.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns <c>true</c> if execution was started, otherwise returns <c>false</c>.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets world context. Only for debugging and testing!
        /// </summary>
        WorldContext WorldContext { get; }

        IMonitor Monitor { get; }
    }
}
