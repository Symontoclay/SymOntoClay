using SymOntoClay.CoreHelper;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.InternalImplementations.BipedNPC;
using SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Place;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Represents core of a game world.
    /// </summary>
    public class WorldCore: ISymOntoClayDisposable
    {
        private static readonly WorldCore __instance = new WorldCore();

        /// <summary>
        /// Gets instance of the class.
        /// </summary>
        public static WorldCore Instance => __instance;

        private WorldCore()
        {
            _context = new WorldContext();
        }

        private readonly WorldContext _context;

        /// <summary>
        /// Sets general settings into the instance.
        /// This method can be run just once in the start of game or application.
        /// You can not use the framework before calling this method.
        /// Calling other methods before this will provoke exceptions.
        /// You can not call the method after disposing the instance.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        public void SetSettings(WorldSettings settings)
        {
            _context.SetSettings(settings);
        }

        /// <summary>
        /// Gets or sets value of enable logging.
        /// It alows enable or disable logging or remote connection for whole components synchronously.
        /// </summary>
        public bool EnableLogging { get => _context.EnableLogging; set => _context.EnableLogging = value; }

        /// <summary>
        /// Gets or sets value of enable remote connection.
        /// It alows enable or disable remote connection for whole components synchronously.
        /// It doesn't touch local logging.
        /// </summary>
        public bool EnableRemoteConnection { get => _context.EnableRemoteConnection; set => _context.EnableRemoteConnection = value; }

        /// <summary>
        /// Registers an instance of NPC by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        public IBipedNPC GetBipedNPC(BipedNPCSettings settings)
        {
            return new BipedNPCImplementation(settings, _context);
        }

        /// <summary>
        /// Registers an instance of Player by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        public IPlayer GetPlayer(PlayerSettings settings)
        {
            return new PlayerImlementation(settings, _context);
        }

        /// <summary>
        /// Registers an instance of game object by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        public IGameObject GetGameObject(GameObjectSettings settings)
        {
            return new GameObjectImplementation(settings, _context);
        }

        /// <summary>
        /// Registers an instance of place by passed settings.
        /// Returned agent provides communication between game level in Unity3D and logical core.
        /// </summary>
        /// <param name="settings">Instance of settings.</param>
        /// <returns>Instance of agent.</returns>
        public IPlace GetPlace(PlaceSettings settings)
        {
            return new PlaceImplementation(settings, _context);
        }

        /// <summary>
        /// Load image by image info.
        /// </summary>
        /// <param name="imageInfo">Instance of image info.</param>
        public void Load(IRunTimeImageInfo imageInfo)
        {
            _context.Load(imageInfo);
        }

        /// <summary>
        /// Load image by Id.
        /// </summary>
        /// <param name="id">Image id.</param>
        public void Load(string id)
        {
            _context.Load(id);
        }

        /// <summary>
        /// Load last image.
        /// </summary>
        public void Load()
        {
            _context.Load();
        }

        /// <summary>
        /// Start execution loaded image.
        /// If there was not loading image default image will be created from source code.
        /// </summary>
        public void Start()
        {
            _context.Start();
        }

        /// <summary>
        /// Stop execution.
        /// </summary>
        public void Stop()
        {
            _context.Stop();
        }

        /// <summary>
        /// Returns <c>true</c> if execution was started, otherwise returns <c>false</c>.
        /// </summary>
        public bool IsActive { get => _context.IsActive; }

        /// <summary>
        /// Creates (saves) image of execured code by settings.
        /// </summary>
        /// <param name="settings">Setting of created (saved) image.</param>
        /// <returns>Instance of image info about created (saved) image.</returns>
        public IRunTimeImageInfo CreateImage(RunTimeImageSettings settings)
        {
            return _context.CreateImage(settings);
        }

        /// <summary>
        /// Creates (saves) image of execured code.
        /// </summary>
        /// <returns>Instance of image info about created (saved) image.</returns>
        public IRunTimeImageInfo CreateImage()
        {
            return _context.CreateImage();
        }

        /// <summary>
        /// Returns image info about last created (saved) image.
        /// </summary>
        public IRunTimeImageInfo CurrentImage { get => _context.CurrentImage; }

        /// <summary>
        /// Returns list of all available image info.
        /// </summary>
        /// <returns>Instnce of list of all available image info.</returns>
        public IList<IRunTimeImageInfo> GetImages()
        {
            return _context.GetImages();
        }

        /// <summary>
        /// Deletes image by image info.
        /// </summary>
        /// <param name="imageInfo">Instance of image info.</param>
        public void DeleteImage(IRunTimeImageInfo imageInfo)
        {
            _context.DeleteImage(imageInfo);
        }

        /// <summary>
        /// Deletes image by Id.
        /// </summary>
        /// <param name="id">Image id.</param>
        public void DeleteImage(string id)
        {
            _context.DeleteImage(id);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _context.Dispose();
        }

        /// <inheritdoc/>
        public bool IsDisposed { get => _context.IsDisposed; }
    }
}
