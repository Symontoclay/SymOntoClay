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
    public class FieldParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotName,
            WaitForType,
            GotType,
            WaitForValue,
            GotValue
        }

        public FieldParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Field Result => _field;
        private Field _field;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _field = CreateField();
            _field.TypeOfAccess = _context.CurrentDefaultSetings.TypeOfAccess;
#if DEBUG
            //Log($"_field = {_field}");
            //Log($"_context.CurrentDefaultSetings.TypeOfAccess = {_context.CurrentDefaultSetings.TypeOfAccess}");
#endif
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"_field = {_field}");
            //Log($"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            _field.Name = ParseName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Var:
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        case TokenKind.Colon:
                            _state = State.WaitForType;
                            break;

                        case TokenKind.Assign:
                            _state = State.WaitForValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForType:
                    _field.TypesList = ParseTypesOfParameterOrVar();
                    _state = State.GotType;
                    break;

                case State.GotType:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        case TokenKind.Assign:
                            _state = State.WaitForValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForValue:
                    {
                        _context.Recovery(_currToken);

                        var parser = new AstExpressionParser(_context, TokenKind.Semicolon);
                        parser.Run();

#if DEBUG
                        //Log($"parser.Result = {parser.Result}");
#endif

                        _field.Value = parser.Result;
                        _state = State.GotValue;
                    }
                    break;

                case State.GotValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
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
