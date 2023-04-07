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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AnnotationSystemEventParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForKindOfEvent,
            GotKindOfEvent,
            GotAsyncMark,
            WaitForAction,
            GotAction
        }

        public AnnotationSystemEventParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AnnotationSystemEvent Result { get; private set; } = new AnnotationSystemEvent();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                    _state = State.WaitForKindOfEvent;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForKindOfEvent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Complete:
                                    Result.Kind = KindOfAnnotationSystemEvent.Complete;
                                    _state = State.GotKindOfEvent;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotKindOfEvent:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.AsyncMarker:
                            Result.IsSync = false;
                            _state = State.GotAsyncMark;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAsyncMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            ProcessFunctionBody();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAction:
                    _context.Recovery(_currToken);
                    Exit();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessFunctionBody()
        {
            _context.Recovery(_currToken);
            var parser = new FunctionBodyParser(_context);
            parser.Run();

            var statementsList = parser.Result;

            Result.Statements = statementsList;
            Result.CompiledFunctionBody = _context.Compiler.Compile(statementsList);

            _state = State.GotAction;
        }
    }
}
