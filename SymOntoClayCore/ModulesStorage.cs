using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class ModulesStorage : IModulesStorage, ISymOntoClayDisposable
    {
        public ModulesStorage(ModulesStorageSettings settings)
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
