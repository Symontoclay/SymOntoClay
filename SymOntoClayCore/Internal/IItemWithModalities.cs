﻿using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface IItemWithModalities
    {
        Value ObligationModality { get; }
        Value SelfObligationModality { get; }
    }
}