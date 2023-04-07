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
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    /// <summary>
    /// It is parser for app.
    /// </summary>
    public class AppPaser : BaseObjectParser
    {
        private enum State
        {
            Init,
            GotApp,
            GotName,
            GotInheritance,
            ContentStarted
        }

        public AppPaser(InternalParserContext context)
            : base(context, KindOfCodeEntity.App)
        {
        }
        
        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch(_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.App:
                            _state = State.GotApp;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotApp:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            Result.Name = ParseName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    ProcessInheritance();
                                    _state = State.GotInheritance;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotInheritance:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
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
