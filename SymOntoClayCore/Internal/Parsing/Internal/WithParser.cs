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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class WithParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForItem,
            GotItemName,
            WaitForItemValue,
            GotItemValue
        }

        public WithParser(CodeItem codeItem, InternalParserContext context, params TerminationToken[] terminators)
            : base(context, terminators)
        {
            _codeItem = codeItem;
        }

        private readonly CodeItem _codeItem;
        private State _state = State.Init;
        private StrongIdentifierValue _itemName;

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
                                case KeyWordTokenKind.With:
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
                            _itemName = NameHelper.CreateName(_currToken.Content);
                            _state = State.GotItemName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotItemName:
                    if (IsValueToken())
                    {
                        ProcessItemValue();
                        break;
                    }
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Assign:
                            _state = State.WaitForItemValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItemValue:
                    if(IsValueToken())
                    {
                        ProcessItemValue();
                        break;
                    }
                    throw new UnexpectedTokenException(_currToken);

                case State.GotItemValue:
                    switch (_currToken.TokenKind)
                    {
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessItemValue()
        {
            var value = ParseValue();

            var nameStr = _itemName?.NormalizedNameValue;

            if (string.IsNullOrWhiteSpace(nameStr))
            {
                throw new Exception("Name of setting of code item can not be null or empty.");
            }

            switch (nameStr)
            {
                case "priority":
                    _codeItem.Priority = value;
                    break;

                default:
                    throw new Exception($"Unexpected name `{nameStr}` of setting of code item.");
            }

            _state = State.GotItemValue;
        }
    }
}
