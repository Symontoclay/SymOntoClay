﻿using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class StandaloneStorage: IStandaloneStorage, ISymOntoClayDisposable
    {
        /// <inheritdoc/>
        public bool IsDisposed => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
