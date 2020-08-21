using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.BipedNPC
{
    /// <inheritdoc/>
    public class BipedNPCImplementation: IBipedNPC
    {
        private readonly BipedNPCGameComponent _gameComponent;

        public BipedNPCImplementation(BipedNPCSettings settings, IWorldCoreGameComponentContext worldContext)
        {
            _gameComponent = new BipedNPCGameComponent(settings, worldContext);
        }

        /// <inheritdoc/>
        public bool EnableLogging { get => _gameComponent.EnableLogging; set => _gameComponent.EnableLogging = value; }

        /// <inheritdoc/>
        public IEntityLogger Logger => _gameComponent.Logger;

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
        public IList<IBipedManualControlledObject> GetManualControlledObjects()
        {
            var initialResultList = _gameComponent.GetManualControlledObjects();

            if(initialResultList.IsNullOrEmpty())
            {
                return new List<IBipedManualControlledObject>();
            }

            var result = new List<IBipedManualControlledObject>();

            foreach(var initialResultItem in initialResultList)
            {
                result.Add(new BipedManualControlledObject(initialResultItem.GameObject, initialResultItem.Devices));
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
