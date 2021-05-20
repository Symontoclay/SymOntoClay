/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
    public class AppPaser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotApp,
            GotName,
            ContentStarted
        }

        public AppPaser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public CodeEntity Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
#if DEBUG
            //Log("Begin");
#endif
            Result = CreateCodeEntity();

            Result.Kind = KindOfCodeEntity.App;
            Result.CodeFile = _context.CodeFile;            

            Result.ParentCodeEntity = CurrentCodeEntity;
            SetCurrentCodeEntity(Result);

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
#if DEBUG
            //Log("Begin");
#endif

            RemoveCurrentCodeEntity();

#if DEBUG
            //Log("End");
#endif
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

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
                                    {
                                        _context.Recovery(_currToken);
                                        var parser = new InheritanceParser(_context, Result.Name);
                                        parser.Run();
                                        Result.InheritanceItems.AddRange(parser.Result);
                                    }
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ContentStarted:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseFigureBracket:
                            Exit();
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.On:
                                    {
                                        _context.Recovery(_currToken);
                                        var parser = new InlineTriggerParser(_context);
                                        parser.Run();
                                        Result.SubItems.Add(parser.Result);
                                    }
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.OpenFactBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new LogicalQueryAsCodeEntityParser(_context);
                                parser.Run();
                                Result.SubItems.Add(parser.Result);
                            }
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
