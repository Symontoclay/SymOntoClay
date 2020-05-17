﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class GlobalStorage: RealStorage
    {
        public GlobalStorage(RealStorageSettings settings)
            : base(KindOfStorage.Global, settings)
        {
        }
    }
}
