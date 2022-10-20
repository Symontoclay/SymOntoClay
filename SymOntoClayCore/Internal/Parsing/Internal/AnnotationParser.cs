/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
    public class AnnotationParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForItem,
            GotSettingsKey,
            WaitForSettingsValue,
            GotItem
        }

        public AnnotationParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public Annotation Result { get; private set; } = new Annotation();
        private StrongIdentifierValue _settingsKey;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result.CheckDirty();

#if DEBUG
            //Log($"Result = {Result}");
#endif
        }

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
                        case TokenKind.OpenAnnotationBracket:
                            _state = State.WaitForItem;
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
                            {
                                var nextToken = _context.GetToken();
                                _context.Recovery(nextToken);

#if DEBUG
                                //Log($"nextToken = {nextToken}");
#endif

                                if(nextToken.TokenKind == TokenKind.Assign)
                                {
                                    _settingsKey = ParseName(_currToken.Content);
                                    _state = State.GotSettingsKey;
                                    break;
                                }

                                Result.MeaningRolesList.Add(ParseName(_currToken.Content));

                                _state = State.GotItem;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSettingsKey:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Assign:
                            _state = State.WaitForSettingsValue;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForSettingsValue:
                    {
                        var parsingResult = ParseValueOnObjDefLevel();

#if DEBUG
                        //Log($"parsingResult = {parsingResult}");
#endif

                        Result.SettingsDict[_settingsKey] = parsingResult.Value;
                        _settingsKey = null;

                        _state = State.GotItem;
                    }
                    break;

                case State.GotItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseAnnotationBracket:
                            Exit();
                            break;

                        case TokenKind.Comma:
                            _state = State.WaitForItem;
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
