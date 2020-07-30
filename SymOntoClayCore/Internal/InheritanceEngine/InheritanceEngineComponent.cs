using SymOntoClay.Core.Internal.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.InheritanceEngine
{
    public class InheritanceEngineComponent: BaseComponent, IInheritanceEngine
    {
        public InheritanceEngineComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;
    }
}
