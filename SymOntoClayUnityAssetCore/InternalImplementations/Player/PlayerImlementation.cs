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

using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Player
{
    /// <inheritdoc/>
    public class PlayerImlementation : IPlayer, IDeferredInitialized
    {
        private PlayerGameComponent _gameComponent;

        public PlayerImlementation(PlayerSettings settings, IWorldCoreGameComponentContext context)
        {
            _gameComponent = new PlayerGameComponent(settings, worldContext);
        }

        public PlayerImlementation(PlayerSettings settings)
        {
            _settings = settings;
        }

        void IDeferredInitialized.Initialize(IWorldCoreGameComponentContext worldContext)
        {
            if (_gameComponent == null)
            {
                _gameComponent = new PlayerGameComponent(_settings, worldContext);
            }
        }

        /// <inheritdoc/>
        public bool EnableLogging { get => _gameComponent.EnableLogging; set => _gameComponent.EnableLogging = value; }

        /// <inheritdoc/>
        public IEntityLogger Logger => _gameComponent.Logger;

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

            if (initialResultList.IsNullOrEmpty())
            {
                return new List<IHumanoidManualControlledObject>();
            }

            var result = new List<IHumanoidManualControlledObject>();

            foreach (var initialResultItem in initialResultList)
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
