/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.HumanoidNPC
{
    /// <inheritdoc/>
    public class HumanoidNPCImplementation: IHumanoidNPC, IDeferredInitialized
    {
        private HumanoidNPCGameComponent _gameComponent;

        public HumanoidNPCImplementation(HumanoidNPCSettings settings, IWorldCoreGameComponentContext worldContext)
        {
            _gameComponent = new HumanoidNPCGameComponent(settings, worldContext);
        }

        public HumanoidNPCImplementation(HumanoidNPCSettings settings)
        {
            _settings = settings;
        }

        void IDeferredInitialized.Initialize(IWorldCoreGameComponentContext worldContext)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _gameComponent = new HumanoidNPCGameComponent(_settings, worldContext);
                }

                if (_addedCategories.Any())
                {
                    _gameComponent.AddCategories(_addedCategories);
                    _addedCategories = null;
                }

                if (_removedCategories.Any())
                {
                    _gameComponent.RemoveCategories(_removedCategories);
                    _removedCategories = null;
                }

                if (_enableCategories.HasValue)
                {
                    _gameComponent.EnableCategories = _enableCategories.Value;
                }
            }
        }

        /// <inheritdoc/>
        public string Id => _gameComponent.Id;

        /// <inheritdoc/>
        public string IdForFacts => _gameComponent.IdForFacts;

        /// <inheritdoc/>
        public int InstanceId => _gameComponent.InstanceId;

        private readonly object _initializeLockObj = new object();

        private readonly HumanoidNPCSettings _settings;

        private List<string> _addedCategories = new List<string>();
        private List<string> _removedCategories = new List<string>();
        private bool? _enableCategories;

        /// <inheritdoc/>
        public bool EnableLogging { get => _gameComponent.EnableLogging; set => _gameComponent.EnableLogging = value; }

        /// <inheritdoc/>
        public IMonitorLogger Logger => _gameComponent.Logger;

        /// <inheritdoc/>
        public void RunInMainThread(Action function)
        {
            _gameComponent.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public TResult RunInMainThread<TResult>(Func<TResult> function)
        {
            return _gameComponent.RunInMainThread(function);
        }

        /// <inheritdoc/>
        public void AddToManualControl(IGameObject obj, DeviceOfBiped device)
        {
            _gameComponent.AddToManualControl(obj, (int)device);
        }

        /// <inheritdoc/>
        public void AddToManualControl(IGameObject obj, IList<DeviceOfBiped> devices)
        {
            _gameComponent.AddToManualControl(obj, devices?.Select(p => (int)p).ToList());
        }

        /// <inheritdoc/>
        public void RemoveFromManualControl(IGameObject obj)
        {
            _gameComponent.RemoveFromManualControl(obj);
        }

        /// <inheritdoc/>
        public IList<IHumanoidManualControlledObject> GetManualControlledObjects()
        {
            var initialResultList = _gameComponent.GetManualControlledObjects();

            if(initialResultList.IsNullOrEmpty())
            {
                return new List<IHumanoidManualControlledObject>();
            }

            var result = new List<IHumanoidManualControlledObject>();

            foreach(var initialResultItem in initialResultList)
            {
                result.Add(new HumanoidManualControlledObject(initialResultItem.GameObject, initialResultItem.Devices));
            }

            return result;
        }

        /// <inheritdoc/>
        public string InsertPublicFact(string text)
        {
            return _gameComponent.InsertPublicFact(text);
        }

        /// <inheritdoc/>
        public string InsertPublicFact(RuleInstance fact)
        {
            return _gameComponent.InsertPublicFact(fact);
        }

        /// <inheritdoc/>
        public void RemovePublicFact(string id)
        {
            _gameComponent.RemovePublicFact(id);
        }

        /// <inheritdoc/>
        public string InsertFact(string text)
        {
            return _gameComponent.InsertFact(text);
        }

        /// <inheritdoc/>
        public void RemoveFact(string id)
        {
            _gameComponent.RemoveFact(id);
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, string text)
        {
            _gameComponent.PushSoundFact(power, text);
        }

        /// <inheritdoc/>
        public void PushSoundFact(float power, RuleInstance fact)
        {
            _gameComponent.PushSoundFact(power, fact);
        }

        /// <inheritdoc/>
        public void AddCategory(string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _addedCategories.Add(category);
                    return;
                }

                _gameComponent.AddCategory(category);
            }
        }

        /// <inheritdoc/>
        public void AddCategories(List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _addedCategories.AddRange(categories);
                    return;
                }

                _gameComponent.AddCategories(categories);
            }
        }

        /// <inheritdoc/>
        public void RemoveCategory(string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _removedCategories.Add(category);
                    return;
                }

                _gameComponent.RemoveCategory(category);
            }
        }

        /// <inheritdoc/>
        public void RemoveCategories(List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _removedCategories.AddRange(categories);
                    return;
                }

                _gameComponent.RemoveCategories(categories);
            }
        }

        /// <inheritdoc/>
        public bool EnableCategories
        {
            get
            {
                lock (_initializeLockObj)
                {
                    if (_gameComponent == null)
                    {
                        return _enableCategories ?? _settings.EnableCategories;
                    }

                    return _gameComponent.EnableCategories;
                }
            }

            set
            {
                lock (_initializeLockObj)
                {
                    if (_gameComponent == null)
                    {
                        _enableCategories = value;
                        return;
                    }

                    _gameComponent.EnableCategories = value;
                }
            }
        }

        /// <inheritdoc/>
        public IStorage BackpackStorage => _gameComponent.BackpackStorage;

        /// <inheritdoc/>
        public void AddToBackpack(IGameObject obj)
        {
            _gameComponent.AddToBackpack(obj);
        }

        /// <inheritdoc/>
        public void RemoveFromBackpack(IGameObject obj)
        {
            _gameComponent.RemoveFromBackpack(obj);
        }

        /// <inheritdoc/>
        public IEngineContext EngineContext => _gameComponent.EngineContext;

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => _gameComponent.StandardFactsBuilder;

        /// <inheritdoc/>
        public void Die()
        {
            _gameComponent.Die();
        }

        /// <inheritdoc/>
        public bool IsDisposed => _gameComponent.IsDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            _gameComponent.Dispose();
        }
    }
}
