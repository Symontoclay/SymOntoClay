using SymOntoClay.Core.Internal.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.LogicalEngine
{
    public class LogicalEngineComponent: BaseComponent, ILogicalEngine
    {
        public LogicalEngineComponent(IParserContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IParserContext _context;
    }
}
