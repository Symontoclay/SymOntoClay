using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations
{
    public class InternalManualControlledObject : IInternalManualControlledObject
    {
        public InternalManualControlledObject(IGameObject gameObject)
        {
            GameObject = gameObject;
        }

        /// <inheritdoc/>
        public IGameObject GameObject { get; private set; }

        /// <inheritdoc/>
        public IList<int> Devices { get; set; } = new List<int>();
    }
}
