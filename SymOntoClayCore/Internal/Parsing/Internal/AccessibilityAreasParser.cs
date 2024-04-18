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
    public class AccessibilityAreasParser: BaseObjectParser
    {
        private enum State
        {
            Init,
            GotAccessibilityAreasMark,
            ContentStarted
        }

        public AccessibilityAreasParser(InternalParserContext context, CodeItem codeItem)
            : base(context, codeItem)
        {
            _codeItem = codeItem;
        }

        private State _state = State.Init;

        private CodeItem _codeItem;

        private TypeOfAccess _prevTypeOfAccess;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _prevTypeOfAccess = _context.CurrentDefaultSetings.TypeOfAccess;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            _context.CurrentDefaultSetings.TypeOfAccess = _prevTypeOfAccess;
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
                                case KeyWordTokenKind.Public:
                                    _context.CurrentDefaultSetings.TypeOfAccess = TypeOfAccess.Public;
                                    _state = State.GotAccessibilityAreasMark;
                                    break;

                                case KeyWordTokenKind.Protected:
                                    _context.CurrentDefaultSetings.TypeOfAccess = TypeOfAccess.Protected;
                                    _state = State.GotAccessibilityAreasMark;
                                    break;

                                case KeyWordTokenKind.Private:
                                    _context.CurrentDefaultSetings.TypeOfAccess = TypeOfAccess.Private;
                                    _state = State.GotAccessibilityAreasMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAccessibilityAreasMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            _state = State.ContentStarted;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ContentStarted:
                    ProcessGeneralContent();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
