using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class SynonymsResolver : BaseResolver
    {
        public SynonymsResolver(IMainStorageContext context)
            : base(context)
        {
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();
        }

        private readonly InheritanceResolver _inheritanceResolver;
    }
}
