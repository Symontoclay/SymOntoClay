/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
