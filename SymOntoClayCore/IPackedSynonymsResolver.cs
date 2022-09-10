﻿using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IPackedSynonymsResolver
    {
        List<StrongIdentifierValue> GetSynonyms(StrongIdentifierValue name);
    }
}
