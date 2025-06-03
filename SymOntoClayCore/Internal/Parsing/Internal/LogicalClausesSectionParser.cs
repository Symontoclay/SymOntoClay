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
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalClausesSectionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotColon,
            GotFact,
            GotBindingVariables
        }

        public LogicalClausesSectionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public List<ActivatingItem> Result { get; private set; }
        private ActivatingItem _currentItem;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new List<ActivatingItem>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            _state = State.GotColon;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotColon:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            ProcessFact();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotFact:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            ProcessFact();
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                case KeyWordTokenKind.Leave:
                                case KeyWordTokenKind.Var:
                                case KeyWordTokenKind.Operator:
                                case KeyWordTokenKind.Fun:
                                case KeyWordTokenKind.Public:
                                case KeyWordTokenKind.Protected:
                                case KeyWordTokenKind.Private:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        case TokenKind.OpenRoundBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new InlineTriggerBindingVariablesParser(_context);
                                parser.Run();

                                _currentItem.BindingVariables = new BindingVariables(parser.Result);

                                _state = State.GotBindingVariables;
                            }
                            break;

                        case TokenKind.Var:
                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotBindingVariables:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            ProcessFact();
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                case KeyWordTokenKind.Leave:
                                case KeyWordTokenKind.Var:
                                case KeyWordTokenKind.Operator:
                                case KeyWordTokenKind.Fun:
                                case KeyWordTokenKind.Public:
                                case KeyWordTokenKind.Protected:
                                case KeyWordTokenKind.Private:
                                    _context.Recovery(_currToken);
                                    Exit();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        case TokenKind.Var:
                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }

        private void ProcessFact()
        {
            _currentItem = new ActivatingItem();
            Result.Add(_currentItem);

            _context.Recovery(_currToken);
            var parser = new LogicalQueryAsCodeEntityParser(_context);
            parser.Run();

            _currentItem.Condition = parser.Result;

            _state = State.GotFact;
        }
    }
}
