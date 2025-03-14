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
            GotRank,
            GotFuzzyLogicNonNumericSequenceItem
        }

        public InheritanceParser(InternalParserContext context, StrongIdentifierValue subName, params TerminationToken[] terminationTokens)
            : base(context, terminationTokens)
        {
            _subName = subName;
        }

        private State _state = State.Init;
        private readonly StrongIdentifierValue _subName;
        public List<InheritanceItem> Result { get; private set; }
        private InheritanceItem _currentItem;
        private FuzzyLogicNonNumericSequenceValue _fuzzyLogicNonNumericSequenceValue;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new List<InheritanceItem>();
            _currentItem = null;
        }

        /// <inheritdoc/>
        protected override bool ShouldBeUsedTerminationToken()
        {
            switch(_state)
            {
                case State.GotSuperName:
                    return true;

                default:
                    return false;
            }
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
                        case TokenKind.Identifier:
                            TryCreateCurrentItem();

                            _currentItem.Rank = new LogicalValue(1.0F);
                            
                            _currentItem.SuperType = ParseName(_currToken.Content).ToTypeInfo();
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

                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                TryCreateCurrentItem();

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

                                        _currentItem.Rank = ParseName(_currToken.Content).ToTypeInfo();

                                        _state = State.GotRankValue;
                                        break;
                                }
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
                            _fuzzyLogicNonNumericSequenceValue.AddIdentifier(ParseName(_currToken.Content));

                            _state = State.GotFuzzyLogicNonNumericSequenceItem;
                            break;

                        case TokenKind.CloseSquareBracket:
                            _currentItem.Rank = _fuzzyLogicNonNumericSequenceValue;
                            _state = State.GotRank;
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
                        case TokenKind.Identifier:
                            TryCreateCurrentItem();

                            _currentItem.SuperType = ParseName(_currToken.Content).ToTypeInfo();
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
                _currentItem.SubType = _subName.ToTypeInfo();
            }
        }
    }
}
