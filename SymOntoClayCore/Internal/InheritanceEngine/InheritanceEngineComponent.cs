using SymOntoClay.Core.Internal.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.InheritanceEngine
{
    public class InheritanceEngineComponent: BaseComponent, IInheritanceEngine
    {
        public InheritanceEngineComponent(IParserContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IParserContext _context;
    }
}
