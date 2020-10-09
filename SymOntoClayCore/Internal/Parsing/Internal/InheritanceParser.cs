/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class InheritanceParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForItem,
            GotSuperName,
            WaitForRank,
            GotRankValue,
            GotRank
        }

        public InheritanceParser(InternalParserContext context, StrongIdentifierValue subName)
            : base(context)
        {
            _subName = subName;
        }

        private State _state = State.Init;
        private readonly StrongIdentifierValue _subName;
        public List<InheritanceItem> Result { get; private set; }
        private InheritanceItem _currentItem;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new List<InheritanceItem>();
            _currentItem = null;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            //Log($"_state = {_state}");
            //Log($"_currentItem = {_currentItem}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    _state = State.WaitForItem;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            TryCreateCurrentItem();

                            _currentItem.Rank = new LogicalValue(1.0F);
                            
                            _currentItem.SuperName = ParseName(_currToken.Content);
                            _state = State.GotSuperName;
                            break;

                        case TokenKind.OpenSquareBracket:
                            _state = State.WaitForRank;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSuperName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _currentItem = null;
                            _state = State.WaitForItem;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForRank:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                TryCreateCurrentItem();

                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context, true);
                                parser.Run();
                                _currentItem.Rank = parser.Result;
                                _state = State.GotRankValue;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotRankValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseSquareBracket:
                            _state = State.GotRank;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotRank:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            TryCreateCurrentItem();

                            _currentItem.SuperName = ParseName(_currToken.Content);
                            _state = State.GotSuperName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void TryCreateCurrentItem()
        {
            if (_currentItem == null)
            {
                _currentItem = CreateInheritanceItem();
                Result.Add(_currentItem);
                _currentItem.SubName = _subName;
            }
        }
    }
}
