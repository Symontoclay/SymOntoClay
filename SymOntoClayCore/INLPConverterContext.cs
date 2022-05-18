using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface INLPConverterContext
    {
        IPackedRelationsResolver RelationsResolver { get; }
        IPackedInheritanceResolver InheritanceResolver { get; }
    }
}
