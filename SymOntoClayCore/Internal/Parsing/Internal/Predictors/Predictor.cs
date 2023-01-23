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

            if (kindOfSpecialPrediction == null)
            {
                switch (currToken.TokenKind)
                {
                    case TokenKind.Word:
                        switch (currToken.KeyWordTokenKind)
                        {
                            case KeyWordTokenKind.Fun:
                                _concretePredictor = new FunctionPredictor(currToken, context);
                                break;

                            case KeyWordTokenKind.New:
                                _concretePredictor = new NewPredictor(currToken, context);
                                break;

                            default:
                                throw new UnexpectedTokenException(currToken);
                        }
                        break;

                    default:
                        throw new UnexpectedTokenException(currToken);
                }
            }
            else
            {
                switch (kindOfSpecialPrediction.Value)
                {
                    case KindOfSpecialPrediction.NamedParameter:
                        _concretePredictor = new NamedParameterPredictor(currToken, context);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfSpecialPrediction), kindOfSpecialPrediction, null);
                }
            }
        }

        private readonly BaseConcretePredictor _concretePredictor;

        public KeyWordTokenKind Predict()
        {
            return _concretePredictor.Predict();
        }
    }
}
