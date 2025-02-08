using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.Factories
{
    public class AppInstanceStorageFactory : IStorageFactory
    {
        /// <inheritdoc/>
        public IStorage CreateStorage(RealStorageSettings settings)
        {
            return new AppInstanceStorage(settings);
        }
    }
}
