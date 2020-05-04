using SymOntoClay.UnityAsset.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.InternalImplementations.Place
{
    /// <inheritdoc/>
    public class PlaceImplementation: IPlace
    {
        public PlaceImplementation(PlaceSettings settings, IWorldCoreGameComponentContext context)
        {

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
