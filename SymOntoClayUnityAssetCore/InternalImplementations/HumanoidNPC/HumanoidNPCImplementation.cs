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

using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
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
                        _gameComponent.DirectInsertPublicFact(Logger, item.Item1, item.Item2);
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
                        _gameComponent.DirectInsertPublicFact(Logger, item);
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
                        _gameComponent.DirectRemovePublicFact(Logger, item);
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
                        _gameComponent.DirectInsertFact(Logger, item.Item1, item.Item2);
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
                        _gameComponent.DirectRemoveFact(Logger, item);
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
                    _gameComponent.DirectAddCategories(null, _deferredAddedCategories);
                    _deferredAddedCategories.Clear();
                    _deferredAddedCategories = null;
                }
                else
                {
                    _deferredAddedCategories = null;
                }

                if (_deferredRemovedCategories.Any())
                {
                    _gameComponent.DirectRemoveCategories(null, _deferredRemovedCategories);
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
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddToManualControl(IGameObject obj, DeviceOfBiped device)
        {
            throw new NotSupportedException("FF96775B-31A1-4BA5-8444-5FB17298523F");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddToManualControl(IGameObject obj, DeviceOfBiped device)
        {
            return _gameComponent.AddToManualControl(obj, (int)device);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddToManualControl(IGameObject obj, IList<DeviceOfBiped> devices)
        {
            throw new NotSupportedException("4FF8E4AC-C6E9-4CEF-8BC7-7BC7B38A22DA");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddToManualControl(IGameObject obj, IList<DeviceOfBiped> devices)
        {
            return _gameComponent.AddToManualControl(obj, devices?.Select(p => (int)p).ToList());
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveFromManualControl(IGameObject obj)
        {
            throw new NotSupportedException("CF47174D-60F6-4FC6-89D3-86C090B4567A");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveFromManualControl(IGameObject obj)
        {
            return _gameComponent.RemoveFromManualControl(obj);
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
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, string text)
        {
            throw new NotSupportedException("091EE105-F536-4C8B-8F6E-2FB54EFC5785");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredPublicFactsTexts.Add((factName, text));
                    return new CompletedSyncMethodResponse<string>(factName.NameValue);
                }

                return _gameComponent.InsertPublicFact(logger, text);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            throw new NotSupportedException("9515CD90-8748-4DB0-9045-B9FBE06B2FB4");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
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
                    return new CompletedSyncMethodResponse<string>(fact.Name.NameValue);
                }

                return _gameComponent.InsertPublicFact(logger, fact);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemovePublicFact(IMonitorLogger logger, string id)
        {
            throw new NotSupportedException("1BB050AC-9062-4246-AB13-329D91A5BA39");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _defferedRemovedPublicFacts.Add(id);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.RemovePublicFact(logger, id);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public string OldInsertFact(IMonitorLogger logger, string text)
        {
            throw new NotSupportedException("8458C937-A928-4467-A79E-523D347B10C4");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertFact(IMonitorLogger logger, string text)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    var factName = NameHelper.CreateRuleOrFactName();
                    _deferredFactsTexts.Add((factName, text));
                    return new CompletedSyncMethodResponse<string>(factName.NameValue);
                }

                return _gameComponent.InsertFact(logger, text);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveFact(IMonitorLogger logger, string id)
        {
            throw new NotSupportedException("1379352D-5020-42DE-BB9F-E9263C83396C");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveFact(IMonitorLogger logger, string id)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _defferedRemovedFacts.Add(id);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.RemoveFact(logger, id);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, string text)
        {
            throw new NotSupportedException("BDE4A752-D659-4C39-B668-F87BF438EB77");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse PushSoundFact(float power, string text)
        {
            return _gameComponent.PushSoundFact(power, text);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldPushSoundFact(float power, RuleInstance fact)
        {
            throw new NotSupportedException("969032FD-9238-4403-A917-8375D1D172D3");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse PushSoundFact(float power, RuleInstance fact)
        {
            return _gameComponent.PushSoundFact(power, fact);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("E2661503-558D-40CA-920D-E1A420205CCA");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredAddedCategories.Add(category);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.AddCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("1DDAA528-869A-41C7-BC6C-4A2DA79138D5");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredAddedCategories.AddRange(categories);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.AddCategories(logger, categories);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategory(IMonitorLogger logger, string category)
        {
            throw new NotSupportedException("0A85FA33-D166-45B1-9643-8349D1C3ED25");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredRemovedCategories.Add(category);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.RemoveCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            throw new NotSupportedException("DFA1FE6C-C757-486A-94E2-3E93554B4F9A");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _deferredRemovedCategories.AddRange(categories);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.RemoveCategories(logger, categories);
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
        [Obsolete("Serialization Refactoring", true)]
        public void OldAddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            throw new NotSupportedException("85468F3C-658C-4942-9B26-411B897E27F5");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddToBackpack(IMonitorLogger logger, IGameObject obj)
        {
            return _gameComponent.AddToBackpack(logger, obj);
        }

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldRemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            throw new NotSupportedException("D2243034-4A17-463B-AB1A-3DBF0F4E4ABC");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveFromBackpack(IMonitorLogger logger, IGameObject obj)
        {
            return _gameComponent.RemoveFromBackpack(logger, obj);
        }

        /// <inheritdoc/>
        public IEngineContext EngineContext => _gameComponent.EngineContext;

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => _gameComponent.StandardFactsBuilder;

        /// <inheritdoc/>
        [Obsolete("Serialization Refactoring", true)]
        public void OldDie()
        {
            throw new NotSupportedException("387F6511-2201-4249-B713-9B546BB83E11");
        }

        /// <inheritdoc/>
        public ISyncMethodResponse Die()
        {
            return _gameComponent.Die();
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
