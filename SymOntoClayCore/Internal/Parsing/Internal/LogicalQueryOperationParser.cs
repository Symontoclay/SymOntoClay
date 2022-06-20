/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalQueryOperationParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotKindOfLogicalQueryOperation,
            GotTarget
        }

        public LogicalQueryOperationParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public LogicalQueryOperationValue Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new LogicalQueryOperationValue();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.QuestionMark:
                            Result.KindOfLogicalQueryOperation = KindOfLogicalQueryOperation.Select;
                            _state = State.GotKindOfLogicalQueryOperation;
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Select:
                                    Result.KindOfLogicalQueryOperation = KindOfLogicalQueryOperation.Select;
                                    _state = State.GotKindOfLogicalQueryOperation;
                                    break;

                                case KeyWordTokenKind.Insert:
                                    Result.KindOfLogicalQueryOperation = KindOfLogicalQueryOperation.Insert;
                                    _state = State.GotKindOfLogicalQueryOperation;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotKindOfLogicalQueryOperation:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new LogicalQueryParser(_context);
                                parser.Run();

                                var ruleInstanceItem = parser.Result;

#if DEBUG
                                //Log($"ruleInstanceItem = {ruleInstanceItem}");
#endif

                                var value = new RuleInstanceValue(ruleInstanceItem);

                                Result.Target = value;

                                _state = State.GotTarget;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotTarget:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.LeftRightStream:
                        case TokenKind.Semicolon:
                            _context.Recovery(_currToken);

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
