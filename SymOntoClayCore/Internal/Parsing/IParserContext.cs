﻿using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public interface IParserContext: IBaseContext
    {
        IEntityDictionary Dictionary { get; }
        ICodeModelContext CodeModelContext { get; }
    }
}
