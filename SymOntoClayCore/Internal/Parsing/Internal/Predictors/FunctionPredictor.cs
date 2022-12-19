using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.Predictors
{
    public class FunctionPredictor: BaseConcretePredictor
    {
        public FunctionPredictor(Token currToken, InternalParserContext context)
            : base(currToken, context)
        {
        }

        /// <inheritdoc/>
        public override KeyWordTokenKind Predict()
        {
            throw new NotImplementedException();
        }
    }
}
