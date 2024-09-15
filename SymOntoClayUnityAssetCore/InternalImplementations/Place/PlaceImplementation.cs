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
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using SymOntoClay.UnityAsset.Core.InternalImplementations.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Place
{
    /// <inheritdoc/>
    public class PlaceImplementation: IPlace, IDeferredInitialized
    {
        public PlaceImplementation(PlaceSettings settings, IWorldCoreGameComponentContext context)
        {
            _gameComponent = new PlaceGameComponent(settings, context);
        }

        public PlaceImplementation(PlaceSettings settings)
        {
            _settings = settings;
        }

        void IDeferredInitialized.Initialize(IWorldCoreGameComponentContext worldContext)
        {
            lock(_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _gameComponent = new PlaceGameComponent(_settings, worldContext);

                    if(_addedCategories.Any())
                    {
                        _gameComponent.DirectAddCategories(null, _addedCategories);
                        _addedCategories = null;
                    }

                    if (_removedCategories.Any())
                    {
                        _gameComponent.DirectRemoveCategories(null, _removedCategories);
                        _removedCategories = null;
                    }

                    if(_enableCategories.HasValue)
                    {
                        _gameComponent.EnableCategories = _enableCategories.Value;
                    }
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

        private PlaceGameComponent _gameComponent;
        private readonly PlaceSettings _settings;

        private List<string> _addedCategories = new List<string>();
        private List<string> _removedCategories = new List<string>();
        private bool? _enableCategories;

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
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _gameComponent.InsertPublicFact(logger, text);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse<string> InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _gameComponent.InsertPublicFact(logger, fact);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemovePublicFact(IMonitorLogger logger, string id)
        {
            return _gameComponent.RemovePublicFact(logger, id);
        }

        /// <inheritdoc/>
        public ISyncMethodResponse PushSoundFact(float power, string text)
        {
            return CompletedSyncMethodResponse.Instance;
        }

        /// <inheritdoc/>
        public ISyncMethodResponse PushSoundFact(float power, RuleInstance fact)
        {
            return CompletedSyncMethodResponse.Instance;
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategory(IMonitorLogger logger, string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _addedCategories.Add(category);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.AddCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        public ISyncMethodResponse AddCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _addedCategories.AddRange(categories);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.AddCategories(logger, categories);
            }
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveCategory(IMonitorLogger logger, string category)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _removedCategories.Add(category);
                    return CompletedSyncMethodResponse.Instance;
                }

                return _gameComponent.RemoveCategory(logger, category);
            }
        }

        /// <inheritdoc/>
        public ISyncMethodResponse RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            lock (_initializeLockObj)
            {
                if (_gameComponent == null)
                {
                    _removedCategories.AddRange(categories);
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
                    if(_gameComponent == null)
                    {
                        _enableCategories = value;
                        return;
                    }

                    _gameComponent.EnableCategories = value;
                }
            }
        }

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
