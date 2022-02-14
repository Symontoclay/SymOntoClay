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

using SymOntoClay.CoreHelper;
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
        void AddConvertor(IPlatformTypesConvertor convertor);

        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets value of enable remote connection.
        /// It alows enable or disable remote connection for whole components synchronously.
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
        /// Loads image by image info.
        /// </summary>
        /// <param name="imageInfo">Instance of image info.</param>
        void Load(IRunTimeImageInfo imageInfo);

        /// <summary>
        /// Loads image by Id.
        /// </summary>
        /// <param name="id">Image id.</param>
        void Load(string id);

        /// <summary>
        /// Loads last image.
        /// </summary>
        void Load();

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
        /// Creates (saves) image of execured code by settings.
        /// </summary>
        /// <param name="settings">Setting of created (saved) image.</param>
        /// <returns>Instance of image info about created (saved) image.</returns>
        IRunTimeImageInfo CreateImage(RunTimeImageSettings settings);

        /// <summary>
        /// Creates (saves) image of execured code.
        /// </summary>
        /// <returns>Instance of image info about created (saved) image.</returns>
        IRunTimeImageInfo CreateImage();

        /// <summary>
        /// Returns image info about last created (saved) image.
        /// </summary>
        IRunTimeImageInfo CurrentImage { get; }

        /// <summary>
        /// Returns list of all available image info.
        /// </summary>
        /// <returns>Instnce of list of all available image info.</returns>
        IList<IRunTimeImageInfo> GetImages();

        /// <summary>
        /// Deletes image by image info.
        /// </summary>
        /// <param name="imageInfo">Instance of image info.</param>
        void DeleteImage(IRunTimeImageInfo imageInfo);

        /// <summary>
        /// Deletes image by Id.
        /// </summary>
        /// <param name="id">Image id.</param>
        void DeleteImage(string id);
    }
}
