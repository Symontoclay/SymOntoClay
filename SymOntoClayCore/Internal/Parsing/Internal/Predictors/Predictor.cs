using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal.Predictors
{
    public class Predictor : BaseLoggedComponent
    {
        public Predictor(Token currToken, InternalParserContext context, KindOfSpecialPrediction? kindOfSpecialPrediction = null)
            : base(context.Logger)
        {
#if DEBUG
            //Log($"currToken = {currToken}");
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
                            if(kindOfSpecialPrediction == null)
                            {
                                throw new UnexpectedTokenException(currToken);
                            }

                            switch(kindOfSpecialPrediction.Value)
                            {
                                case KindOfSpecialPrediction.NamedParameter:
                                    _concretePredictor = new NamedParameterPredictor(currToken, context);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(kindOfSpecialPrediction), kindOfSpecialPrediction, null);
                            }
                            break;
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
