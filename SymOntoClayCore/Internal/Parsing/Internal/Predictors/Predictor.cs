/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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
