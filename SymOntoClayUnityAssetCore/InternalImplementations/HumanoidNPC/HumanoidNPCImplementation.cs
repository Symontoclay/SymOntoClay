/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
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
            if(_gameComponent == null)
            {
                _gameComponent = new HumanoidNPCGameComponent(_settings, worldContext);
            }
        }

        private readonly HumanoidNPCSettings _settings;

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
        public bool IsDisposed => _gameComponent.IsDisposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            _gameComponent.Dispose();
        }
    }
}
