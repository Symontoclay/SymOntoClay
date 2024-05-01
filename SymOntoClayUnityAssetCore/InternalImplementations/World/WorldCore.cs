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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.InternalImplementations;
using SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject;
using SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Place;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Player;
using System;
using System.Collections.Generic;

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
        public string Id => string.Empty;

        /// <inheritdoc/>
        public string IdForFacts => string.Empty;

        /// <inheritdoc/>
        public int InstanceId => 0;

        /// <inheritdoc/>
        public WorldContext WorldContext => _context;

        /// <inheritdoc/>
        public IMonitor Monitor => _context.Monitor;

        /// <inheritdoc/>
        public void SetSettings(WorldSettings settings)
        {
            lock(_lockObj)
            {
                _context.SetSettings(settings);

                InitializeDeferred();
            }
        }

        /// <inheritdoc/>
        public void AddConvertor(IPlatformTypesConverter convertor)
        {
            lock (_lockObj)
            {
                if (_context.IsInitialized)
                {
                    _context.AddConvertor(convertor);
                }
                else
                {
                    _deferredPlatformTypesConvertorsList.Add(convertor);
                }
            }                
        }

        /// <inheritdoc/>
        public bool EnableLogging { get => _context.EnableLogging; set => _context.EnableLogging = value; }

        /// <inheritdoc/>
        public bool EnableRemoteConnection { get => _context.EnableRemoteConnection; set => _context.EnableRemoteConnection = value; }

        /// <inheritdoc/>
        public IMonitorLogger Logger => _context.Logger;

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
        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, string text)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, RuleInstance fact)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddCategory(IMonitorLogger logger, string category)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool EnableCategories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
        private readonly object _lockObj = new object();

        private readonly WorldContext _context;

        private void AddToDeferredInitialization(IDeferredInitialized deferredInitialized)
        {
            _deferredInitializedList.Add(deferredInitialized);
        }

        private void InitializeDeferred()
        {
            if(!_deferredPlatformTypesConvertorsList.IsNullOrEmpty())
            {
                foreach(var deferredPlatformTypesConvertor in _deferredPlatformTypesConvertorsList)
                {
                    _context.AddConvertor(deferredPlatformTypesConvertor);
                }                
            }

            foreach(var deferredInitialized in _deferredInitializedList)
            {
                deferredInitialized.Initialize(_context);
            }
        }

        private List<IDeferredInitialized> _deferredInitializedList = new List<IDeferredInitialized>();

        private List<IPlatformTypesConverter> _deferredPlatformTypesConvertorsList = new List<IPlatformTypesConverter>();
        #endregion
    }
}
