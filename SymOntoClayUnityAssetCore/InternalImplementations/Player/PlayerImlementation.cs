using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Player
{
    /// <inheritdoc/>
    public class PlayerImlementation : IPlayer
    {
        public PlayerImlementation(PlayerSettings settings, IWorldCoreGameComponentContext context)
        {

        }

        /// <inheritdoc/>
        public ILogger Logger => throw new NotImplementedException();

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
