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

using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.SerializationToImage.ComponentsInterfaces;
using SymOntoClay.Monitor.Common;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.GameObject
{
    /// <inheritdoc/>
    public class GameObjectImplementation: IGameObject, ISerializedWorldComponent
    {
        public GameObjectImplementation(GameObjectSettings settings, IWorldCoreGameComponentContext context)
        {
            _settings = settings;

            _gameComponent = new GameObjectGameComponent(settings, context);
        }

        /// <inheritdoc/>
        public string Id => _gameComponent.Id;

        /// <inheritdoc/>
        public string IdForFacts => _gameComponent.IdForFacts;

        /// <inheritdoc/>
        public int InstanceId => _gameComponent.InstanceId;

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
        public string InsertPublicFact(IMonitorLogger logger, string text)
        {
            return _gameComponent.InsertPublicFact(logger, text);
        }

        /// <inheritdoc/>
        public string InsertPublicFact(IMonitorLogger logger, RuleInstance fact)
        {
            return _gameComponent.InsertPublicFact(logger, fact);
        }

        /// <inheritdoc/>
        public void RemovePublicFact(IMonitorLogger logger, string id)
        {
            _gameComponent.RemovePublicFact(logger, id);
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
            _gameComponent.AddCategory(logger, category);
        }

        /// <inheritdoc/>
        public void AddCategories(IMonitorLogger logger, List<string> categories)
        {
            _gameComponent.AddCategories(logger, categories);
        }

        /// <inheritdoc/>
        public void RemoveCategory(IMonitorLogger logger, string category)
        {
            _gameComponent.RemoveCategory(logger, category);
        }

        /// <inheritdoc/>
        public void RemoveCategories(IMonitorLogger logger, List<string> categories)
        {
            _gameComponent.RemoveCategories(logger, categories);
        }

        /// <inheritdoc/>
        public bool EnableCategories
        {
            get
            {
                return _gameComponent.EnableCategories;
            }

            set
            {
                _gameComponent.EnableCategories = value;
            }
        }

        /// <inheritdoc/>
        public IStorage PublicFactsStorage => _gameComponent.PublicFactsStorage;

        [SettingsMember]
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
