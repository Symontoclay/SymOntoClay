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

        public KeyWordTokenKind Result { get; private set; } = KeyWordTokenKind.Unknown;

        protected virtual KeyWordTokenKind DefaultResult { get; set; } = KeyWordTokenKind.Unknown;

        protected void Complete()
        {
            Complete(DefaultResult);
        }

        protected void Complete(KeyWordTokenKind result)
        {
            Result = result;
            Exit();
        }

        public KeyWordTokenKind Predict()
        {
            Run();
            return Result;
        }
    }
}
