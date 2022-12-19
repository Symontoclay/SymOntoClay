using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.Predictors
{
    public abstract class BaseConcretePredictor: BaseLoggedComponent
    {
        protected BaseConcretePredictor(Token currToken, InternalParserContext context)
            : base(context.Logger)
        {
            _context = context.Fork();
            _context.Recovery(currToken);
        }

        private readonly InternalParserContext _context;

        public abstract KeyWordTokenKind Predict();
    }
}
