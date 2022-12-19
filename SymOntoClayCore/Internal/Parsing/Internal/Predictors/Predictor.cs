using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.Predictors
{
    public class Predictor : BaseLoggedComponent
    {
        public Predictor(Token currToken, InternalParserContext context)
            : base(context.Logger)
        {
#if DEBUG
            Log($"currToken = {currToken}");
#endif

            switch(currToken.TokenKind)
            {
                case TokenKind.Word:
                    switch(currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Fun:
                            _concretePredictor = new FunctionPredictor(currToken, context);
                            break;

                        default:
                            throw new UnexpectedTokenException(currToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(currToken);
            }
        }

        private readonly BaseConcretePredictor _concretePredictor;

        public KeyWordTokenKind Predict()
        {
            return _concretePredictor.Predict();
        }
    }
}
