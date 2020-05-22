using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Serialization
{
    public interface IBaseLoaderFromSourceCodeContext: IParserContext
    {
        IParser Parser { get; }
    }
}
