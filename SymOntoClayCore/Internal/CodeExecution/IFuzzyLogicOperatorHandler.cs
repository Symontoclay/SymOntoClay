﻿using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public interface IFuzzyLogicOperatorHandler : IFuzzyLogicHandler
    {
        KindOfFuzzyLogicOperatorFunction Kind { get; }
    }
}