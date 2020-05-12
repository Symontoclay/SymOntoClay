using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.BipedNPC
{
    /// <inheritdoc/>
    public class BipedNPCImplementation: IBipedNPC
    {
        private readonly BipedNPCGameComponent _gameComponent;

        public BipedNPCImplementation(BipedNPCSettings settings, IWorldCoreGameComponentContext context)
        {
            _gameComponent = new BipedNPCGameComponent(settings, context);
        }

        /// <inheritdoc/>
        public bool EnableLogging { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public IEntityLogger Logger => _gameComponent.Logger;

        /// <inheritdoc/>
        public void AddToManualControl(IGameObject obj, DeviceOfBiped device)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void AddToManualControl(IGameObject obj, IList<DeviceOfBiped> devices)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveFromManualControl(IGameObject obj)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IList<IManualControlledObject> GetManualControlledObjects()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IsDisposed => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
