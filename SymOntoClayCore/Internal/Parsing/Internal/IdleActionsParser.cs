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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class IdleActionsParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotIdleMark,
            GotActionsMark,
            WaitForItems,
            GotItem
        }

        public IdleActionsParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public List<IdleActionItem> Result { get; private set; }
        private IdleActionItem _currentItem;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new List<IdleActionItem>();
            _currentItem = null;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Idle:
                                    _state = State.GotIdleMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIdleMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Actions:
                                    _state = State.GotActionsMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotActionsMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.WaitForItems;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItems:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            ProcessCodeExpression();
                            break;

                        case TokenKind.OpenFactBracket:
                            ProcessFactExpression();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            ProcessCodeExpression();
                            break;

                        case TokenKind.OpenFactBracket:
                            ProcessFactExpression();
                            break;

                        case TokenKind.CloseFigureBracket:
                            Exit();
                            break;

                        case TokenKind.OpenAnnotationBracket:
                            ProcessAnnotation();
                            break;

                        case TokenKind.Semicolon:
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessAnnotation()
        {
            _context.Recovery(_currToken);

            var parser = new AnnotationParser(_context);
            parser.Run();

            _currentItem.RuleInstance.AddAnnotation(parser.Result);
        }

        private void ProcessCodeExpression()
        {
            _context.Recovery(_currToken);
            var parser = new CodeExpressionStatementParser(_context);
            parser.Run();

            _currentItem = CreateIdleActionItem();
            Result.Add(_currentItem);

            _currentItem.Statements.Add(parser.Result);

            _currentItem.CompiledFunctionBody = _context.Compiler.Compile(_currentItem.Statements);

            _state = State.GotItem;
        }

        private void ProcessFactExpression()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var ruleInstanceItem = parser.Result;

            _currentItem = CreateIdleActionItem();
            Result.Add(_currentItem);

            _currentItem.RuleInstance = ruleInstanceItem;

            _state = State.GotItem;
        }
    }
}
