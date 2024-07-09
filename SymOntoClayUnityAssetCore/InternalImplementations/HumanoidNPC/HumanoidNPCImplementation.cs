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
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Serialization.Functors;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

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

                if(_deferredPublicFactsTexts.Any())
                {
                    foreach(var item in _deferredPublicFactsTexts)
                    {
                        _gameComponent.InsertPublicFact(Logger, item.Item1, item.Item2);
                    }

                    _deferredPublicFactsTexts.Clear();
                    _deferredPublicFactsTexts = null;
                }
                else
                {
                    _deferredPublicFactsTexts = null;
                }

                if (_deferredPublicFactsInstances.Any())
                {
                    foreach (var item in _deferredPublicFactsInstances)
                    {
                        _gameComponent.InsertPublicFact(Logger, item);
                    }

                    _deferredPublicFactsInstances.Clear();
                    _deferredPublicFactsInstances = null;
                }
                else
                {
                    _deferredPublicFactsInstances = null;
                }

                if (_defferedRemovedPublicFacts.Any())
                {
                    foreach (var item in _defferedRemovedPublicFacts)
                    {
                        _gameComponent.RemovePublicFact(Logger, item).Wait();
                    }

                    _defferedRemovedPublicFacts.Clear();
                    _defferedRemovedPublicFacts = null;
                }
                else
                {
                    _defferedRemovedPublicFacts = null;
                }

                if (_deferredFactsTexts.Any())
                {
                    foreach (var item in _deferredFactsTexts)
                    {
                        _gameComponent.InsertFact(Logger, item.Item1, item.Item2);
                    }

                    _deferredFactsTexts.Clear();
                    _deferredFactsTexts = null;
                }
                else
                {
                    _deferredFactsTexts = null;
                }

                if (_defferedRemovedFacts.Any())
                {
                    foreach (var item in _defferedRemovedFacts)
                    {
                        _gameComponent.RemoveFact(Logger, item);
                    }

                    _defferedRemovedFacts.Clear();
                    _defferedRemovedFacts = null;
                }
                else
                {
                    _defferedRemovedFacts = null;
                }

                if (_deferredAddedCategories.Any())
                {
                    _gameComponent.AddCategories(null, _deferredAddedCategories);
                    _deferredAddedCategories.Clear();
                    _deferredAddedCategories = null;
                }
                else
                {
                    _deferredAddedCategories = null;
                }

                if (_deferredRemovedCategories.Any())
                {
                    _gameComponent.RemoveCategories(null, _deferredRemovedCategories);
                    _deferredRemovedCategories.Clear();
                    _deferredRemovedCategories = null;
                }
                else
                {
                    _deferredRemovedCategories = null;
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

        private List<(StrongIdentifierValue, string)> _deferredPublicFactsTexts = new List<(StrongIdentifierValue, string)>();
        private List<RuleInstance> _deferredPublicFactsInstances = new List<RuleInstance>();
        private List<string> _defferedRemovedPublicFacts = new List<string>();
        private List<(StrongIdentifierValue, string)> _deferredFactsTexts = new List<(StrongIdentifierValue, string)>();
        private List<string> _defferedRemovedFacts = new List<string>();
        private List<string> _deferredAddedCategories = new List<string>();
        private List<string> _deferredRemovedCategories = new List<string>();
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
        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredPublicFactsTexts.Add((factName, text));
                    return factName.NameValue;
                }

                return _gameComponent.InsertPublicFact(logger, text);
            }
        }

        /// <inheritdoc/>
        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    if (fact.Name == null)
                    {
                        fact.Name = NameHelper.CreateRuleOrFactName();
                    }

                    _deferredPublicFactsInstances.Add(fact);
                    return fact.Name.NameValue;
                }

                return _gameComponent.InsertPublicFact(logger, fact);
            }
        }

        /// <inheritdoc/>
        public IMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _defferedRemovedPublicFacts.Add(id);
                    return CompletedMethodResponse.Instance;
                }

                return _gameComponent.RemovePublicFact(logger, id);
            }
        }

        /// <inheritdoc/>
        public string InsertFact(IMonitorLogger logger, string text)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredFactsTexts.Add((factName, text));
                    return factName.NameValue;
                }

                return _gameComponent.InsertFact(logger, text);
            }
        }

        /// <inheritdoc/>
        public void RemoveFact(IMonitorLogger logger, string id)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _defferedRemovedFacts.Add(id);
                    return;
                }

                _gameComponent.RemoveFact(logger, id);
            }
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
        public void AddCategory(IMonitorLogger logger, string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredAddedCategories.Add(category);
                    return;
                }

                _gameComponent.AddCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredAddedCategories.AddRange(categories);
                    return;
                }

                _gameComponent.AddCategories(logger, categories);
            }
        }

        /// <inheritdoc/>
        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredRemovedCategories.Add(category);
                    return;
                }

                _gameComponent.RemoveCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredRemovedCategories.AddRange(categories);
                    return;
                }

                _gameComponent.RemoveCategories(logger, categories);
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
        public void AddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _gameComponent.AddToBackpack(logger, obj);
        }

        /// <inheritdoc/>
        public void RemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            _gameComponent.RemoveFromBackpack(logger, obj);
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
