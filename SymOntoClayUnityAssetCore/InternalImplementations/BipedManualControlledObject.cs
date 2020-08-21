using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public class BipedManualControlledObject : IBipedManualControlledObject
    {
        public BipedManualControlledObject(IGameObject gameObject, IList<int> devices)
        {
            _gameObject = gameObject;
            _devices = devices?.Select(p => (DeviceOfBiped)p).ToList();
        }

        private readonly IGameObject _gameObject;
        private readonly IList<DeviceOfBiped> _devices;

        /// <inheritdoc/>
        public IGameObject GameObject => _gameObject;

        /// <inheritdoc/>
        public IList<DeviceOfBiped> Devices => _devices;
    }
}
