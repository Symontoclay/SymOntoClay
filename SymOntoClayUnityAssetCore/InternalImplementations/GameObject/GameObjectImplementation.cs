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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject
{
    /// <inheritdoc/>
    public class GameObjectImplementation: IGameObject, IDeferredInitialized
    {
        public GameObjectImplementation(GameObjectSettings settings, IWorldCoreGameComponentContext context)
        {
            _gameComponent = new GameObjectGameComponent(settings, context);
        }

        public GameObjectImplementation(GameObjectSettings settings)
        {
            _settings = settings;
        }

        void IDeferredInitialized.Initialize(IWorldCoreGameComponentContext worldContext)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _gameComponent = new GameObjectGameComponent(_settings, worldContext);

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
        }

        private readonly object _initializeLockObj = new object();
        private List<string> _addedCategories = new List<string>();
        private List<string> _removedCategories = new List<string>();
        private bool? _enableCategories;

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
        public IStorage PublicFactsStorage => _gameComponent.PublicFactsStorage;

        private readonly GameObjectSettings _settings;

        private GameObjectGameComponent _gameComponent;

        /// <inheritdoc/>
        public IEndpointsRegistry EndpointsRegistry => _gameComponent.EndpointsRegistry;

        /// <inheritdoc/>
        public IMonitorLogger Logger => _gameComponent.Logger;

        /// <inheritdoc/>
        public IStandardFactsBuilder StandardFactsBuilder => _gameComponent.StandardFactsBuilder;

        /// <inheritdoc/>
        public bool IsDisposed => _gameComponent.IsDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            _gameComponent.Dispose();
        }
    }
}
