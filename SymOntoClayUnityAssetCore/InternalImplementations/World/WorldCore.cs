/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
using SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Place;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Player;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.UnityAsset.Core.InternalImplementations;

namespace SymOntoClay.UnityAsset.Core.World
{
    public class WorldCore: IWorld
    {
        #region constructors
        public WorldCore()
        {
            _context = new WorldContext();
        }
        #endregion

        #region public members
        /// <inheritdoc/>
        public void SetSettings(WorldSettings settings)
        {
            lock(_lockObj)
            {
#if DEBUG
                //_logger.Info("SetSettings");
#endif

                _context.SetSettings(settings);

                InitializeDeferred();
            }
        }

        public void AddConvertor(IPlatformTypesConvertor convertor)
        {
            lock (_lockObj)
            {
                if (_context.IsInitialized)
                {
                    _context.AddConvertor(convertor);
                }
                else
                {
                    _deferredPlatformTypesConvertor = convertor;
                }
            }                
        }

        /// <inheritdoc/>
        public bool EnableLogging { get => _context.EnableLogging; set => _context.EnableLogging = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => _context.EnableRemoteConnection; set => _context.EnableRemoteConnection = value; }

        /// <inheritdoc/>
        public IEntityLogger Logger => _context.Logger;

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            _context.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _context.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public IHumanoidNPC GetHumanoidNPC(HumanoidNPCSettings settings)
        {
            lock (_lockObj)
            {
#if DEBUG
                //_logger.Info($"GetHumanoidNPC _context.IsInitialized = {_context.IsInitialized}");
#endif

                if (_context.IsInitialized)
                {
                    return new HumanoidNPCImplementation(settings, _context);
                }

                var result = new HumanoidNPCImplementation(settings);
                AddToDeferredInitialization(result);

                return result;
            }
        }

        /// <inheritdoc/>
        public IPlayer GetPlayer(PlayerSettings settings)
        {
            if (_context.IsInitialized)
            {
                return new PlayerImlementation(settings, _context);
            }

            var result = new PlayerImlementation(settings);
            AddToDeferredInitialization(result);

            return result;
        }

        /// <inheritdoc/>
        public IGameObject GetGameObject(GameObjectSettings settings)
        {
            if (_context.IsInitialized)
            {
                return new GameObjectImplementation(settings, _context);
            }

            var result = new GameObjectImplementation(settings);
            AddToDeferredInitialization(result);

            return result;
        }

        /// <inheritdoc/>
        public IPlace GetPlace(PlaceSettings settings)
        {
            if (_context.IsInitialized)
            {
                return new PlaceImplementation(settings, _context);
            }

            var result = new PlaceImplementation(settings);
            AddToDeferredInitialization(result);

            return result;
        }

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Load(IRunTimeImageInfo imageInfo)
        {
            _context.Load(imageInfo);
        }

        /// <inheritdoc/>
        public void Load(string id)
        {
            _context.Load(id);
        }

        /// <inheritdoc/>
        public void Load()
        {
            _context.Load();
        }

        /// <inheritdoc/>
        public void Start()
        {
            _context.Start();
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _context.Stop();
        }

        /// <inheritdoc/>
        public bool IsActive { get => _context.IsActive; }

        /// <inheritdoc/>
        public IRunTimeImageInfo CreateImage(RunTimeImageSettings settings)
        {
            return _context.CreateImage(settings);
        }

        /// <inheritdoc/>
        public IRunTimeImageInfo CreateImage()
        {
            return _context.CreateImage();
        }

        /// <inheritdoc/>
        public IRunTimeImageInfo CurrentImage { get => _context.CurrentImage; }

        /// <inheritdoc/>
        public IList<IRunTimeImageInfo> GetImages()
        {
            return _context.GetImages();
        }

        /// <inheritdoc/>
        public void DeleteImage(IRunTimeImageInfo imageInfo)
        {
            _context.DeleteImage(imageInfo);
        }

        /// <inheritdoc/>
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
        #endregion

        #region private members
#if DEBUG
        //private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        private readonly object _lockObj = new object();

        private readonly WorldContext _context;

        private void AddToDeferredInitialization(IDeferredInitialized deferredInitialized)
        {
            _deferredInitializedList.Add(deferredInitialized);
        }

        private void InitializeDeferred()
        {
            if(_deferredPlatformTypesConvertor != null)
            {
                _context.AddConvertor(_deferredPlatformTypesConvertor);
            }

            foreach(var deferredInitialized in _deferredInitializedList)
            {
                deferredInitialized.Initialize(_context);
            }
        }

        private List<IDeferredInitialized> _deferredInitializedList = new List<IDeferredInitialized>();

        private IPlatformTypesConvertor _deferredPlatformTypesConvertor;
        #endregion
    }
}
