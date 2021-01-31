﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IBaseCoreContext
    {
        IEntityLogger Logger { get; }
        IEntityDictionary Dictionary { get; }
    }
}