using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface INLPConverter
    {
        IList<RuleInstance> Convert(string text);
        string Convert(RuleInstance fact, IStorage storage);
    }
}
