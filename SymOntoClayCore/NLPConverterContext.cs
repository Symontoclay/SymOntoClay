using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class NLPConverterContext : INLPConverterContext
    {
        public NLPConverterContext()
        {
        }

        public NLPConverterContext(IPackedRelationsResolver relationsResolver, IPackedInheritanceResolver inheritanceResolver)
        {
            RelationsResolver = relationsResolver;
            InheritanceResolver = inheritanceResolver;
        }

        public IPackedRelationsResolver RelationsResolver { get; set; }
        public IPackedInheritanceResolver InheritanceResolver { get; set; }
    }
}
