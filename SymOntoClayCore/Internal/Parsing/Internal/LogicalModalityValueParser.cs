/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalModalityValueParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotFuzzyLogicNonNumericSequenceItem,
            GotExpression
        }

        public LogicalModalityValueParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Value Result { get; private set; }

        private FuzzyLogicNonNumericSequenceValue _fuzzyLogicNonNumericSequenceValue;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context, true);
                                parser.Run();

                                Result = parser.Result;

                                Exit();
                            }
                            break;

                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                var kindOfRuleInstanceSectionMark = PeekKindOfRuleInstanceSectionMark();

                                if(kindOfRuleInstanceSectionMark != KindOfRuleInstanceSectionMark.Unknown)
                                {
                                    throw new UnexpectedTokenException(_currToken);
                                }

                                var nextToken = _context.GetToken();

#if DEBUG

#endif
                                switch (nextToken.TokenKind)
                                {
                                    case TokenKind.Word:
                                    case TokenKind.Identifier:
                                        _context.Recovery(nextToken);

                                        _fuzzyLogicNonNumericSequenceValue = new FuzzyLogicNonNumericSequenceValue();
                                        _fuzzyLogicNonNumericSequenceValue.AddIdentifier(ParseName(_currToken.Content));

                                        _state = State.GotFuzzyLogicNonNumericSequenceItem;
                                        break;

                                    default:
                                        _context.Recovery(nextToken);

                                        Result = ParseName(_currToken.Content);

                                        Exit();
                                        break;
                                }                                
                            }
                            break;

                        case TokenKind.OpenFigureBracket:
                            {
                                var parser = new LogicalModalityExpressionParser(_context, TokenKind.CloseFigureBracket);
                                parser.Run();

                                Result = new LogicalModalityExpressionValue() { Expression = parser.Result };


                                _state = State.GotExpression;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFuzzyLogicNonNumericSequenceItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            {
                                var kindOfRuleInstanceSectionMark = PeekKindOfRuleInstanceSectionMark();

                                if(kindOfRuleInstanceSectionMark == KindOfRuleInstanceSectionMark.Unknown)
                                {
                                    _fuzzyLogicNonNumericSequenceValue.AddIdentifier(ParseName(_currToken.Content));

                                    _state = State.GotFuzzyLogicNonNumericSequenceItem;

                                    break;
                                }

                                _context.Recovery(_currToken);

                                if(_fuzzyLogicNonNumericSequenceValue.NonNumericValue != null && _fuzzyLogicNonNumericSequenceValue.Operators.IsNullOrEmpty())
                                {
                                    Result = _fuzzyLogicNonNumericSequenceValue.NonNumericValue;

                                    Exit();
                                    break;
                                }
                                

                                Result = _fuzzyLogicNonNumericSequenceValue;

                                Exit();
                            }
                            break;

                        case TokenKind.CloseFactBracket:
                            Result = _fuzzyLogicNonNumericSequenceValue;

                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotExpression:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFigureBracket:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
