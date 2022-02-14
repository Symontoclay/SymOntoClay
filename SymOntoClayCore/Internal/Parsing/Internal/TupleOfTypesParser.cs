/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
    public class TupleOfTypesParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForTypeIdentifier,
            GotTypeIdentifier
        }

        public TupleOfTypesParser(InternalParserContext context, bool boundedBuRoundBrackets)
            : base(context)
        {
            _boundedBuRoundBrackets = boundedBuRoundBrackets;

            if(boundedBuRoundBrackets)
            {
                _state = State.Init;
            }
            else
            {
                _state = State.WaitForTypeIdentifier;
            }
        }

        private readonly bool _boundedBuRoundBrackets;

        private State _state = State.Init;

        public List<StrongIdentifierValue> Result { get; set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForTypeIdentifier;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForTypeIdentifier:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                Result.Add(ParseName(_currToken.Content));

                                _state = State.GotTypeIdentifier;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotTypeIdentifier:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Or:
                            _state = State.WaitForTypeIdentifier;
                            break;

                        case TokenKind.CloseRoundBracket:
                            if(!_boundedBuRoundBrackets)
                            {
                                _context.Recovery(_currToken);
                            }
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
