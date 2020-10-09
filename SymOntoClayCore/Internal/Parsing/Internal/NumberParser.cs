/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class NumberParser : BaseInternalParser
    {
        private readonly static CultureInfo _targetCulture = new CultureInfo("en-GB");

        private enum State
        {
            Init,
            GotIntegerPart,
            GotSeparator,
            GotFractionPart
        }

        public NumberParser(InternalParserContext context, bool createLogicalValue = false)
            : base(context)
        {
            _createLogicalValue = createLogicalValue;
        }

        private State _state = State.Init;

        public Value Result { get; private set; }
        public StringBuilder _buffer;
        private readonly bool _createLogicalValue;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _buffer = new StringBuilder();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"buffer = {_buffer}");
            //Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            _state = State.GotIntegerPart;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIntegerPart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            break;

                        case TokenKind.Point:
                            {
                                var nextToken = _context.GetToken();

#if DEBUG
                                //Log($"nextToken = {nextToken}");
#endif

                                if(nextToken.TokenKind == TokenKind.Number)
                                {
                                    _context.Recovery(nextToken);

                                    _buffer.Append(".");

                                    _state = State.GotSeparator;
                                }
                                else
                                {
                                    _context.Recovery(nextToken);
                                    _context.Recovery(_currToken);
                                    Exit();
                                }                              
                            }
                            break;

                        case TokenKind.CloseSquareBracket:
                            ProcessCloseSquareBracket();
                            break;

                        case TokenKind.Comma:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.Semicolon:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSeparator:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            _state = State.GotFractionPart;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFractionPart:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            _buffer.Append(_currToken.Content);
                            break;

                        case TokenKind.CloseSquareBracket:
                            ProcessCloseSquareBracket();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessCloseSquareBracket()
        {
            _context.Recovery(_currToken);
            Exit();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
#if DEBUG
            //Log($"buffer = {_buffer}");
#endif

            if(_createLogicalValue)
            {
                var numberValue = float.Parse(_buffer.ToString(), _targetCulture);

#if DEBUG
                //Log($"numberValue = {numberValue}");
#endif

                Result = new LogicalValue(numberValue);
            }
            else
            {
                var numberValue = double.Parse(_buffer.ToString(), _targetCulture);

#if DEBUG
                //Log($"numberValue = {numberValue}");
#endif

                Result = new NumberValue(numberValue);
            }
        }
    }
}
