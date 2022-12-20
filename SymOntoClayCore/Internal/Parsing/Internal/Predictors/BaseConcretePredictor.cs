using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.Predictors
{
    public abstract class BaseConcretePredictor: BaseInternalParser
    {
        private static InternalParserContext PrepareContext(Token currToken, InternalParserContext context)
        {
            var newContext = context.Fork();
            newContext.Recovery(currToken);

            return newContext;
        }

        protected BaseConcretePredictor(Token currToken, InternalParserContext context)
            : base(PrepareContext(currToken, context))
        {            
        }

        public abstract KeyWordTokenKind Predict();
    }
}
