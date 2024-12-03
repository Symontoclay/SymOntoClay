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

        private App _app;

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            base.OnEnter();

            _app = (App)Result;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Info("55063C1F-985F-4F33-8374-7760F707CA31", $"55063C1F-985F-4F33-8374-7760F707CA31 _state = {_state}");
            Info("A3725C22-C133-468E-B62B-01D2631FB2C9", $"A3725C22-C133-468E-B62B-01D2631FB2C9 _currToken = {_currToken}");
            //Info("AE8EB47A-1E18-4E4D-9C95-7188537DA7F0", $"AE8EB47A-1E18-4E4D-9C95-7188537DA7F0 Result = {Result}");
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
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Root:
                                    {
                                        var nextToken = _context.GetToken();

#if DEBUG
                                        Info("CFAC1423-B051-46F7-8ECA-B1064798B08B", $"nextToken = {nextToken}");
#endif

                                        switch (nextToken.TokenKind)
                                        {
                                            case TokenKind.Word:
                                                switch (nextToken.KeyWordTokenKind)
                                                {
                                                    case KeyWordTokenKind.Task:
                                                        {
                                                            _context.Recovery(nextToken);
                                                            _context.Recovery(_currToken);

                                                            var parser = new SetRootTaskParser(_context);
                                                            parser.Run();

#if DEBUG
                                                            Info("F6CA06F0-0631-4FBD-A070-5CDD9C6B1838", $"parser.Result = {parser.Result}");
#endif

                                                            _app.RootTasks.Add(parser.Result);
                                                        }
                                                        break;

                                                    default:
                                                        throw new UnexpectedTokenException(nextToken);
                                                }
                                                break;

                                            default:
                                                throw new UnexpectedTokenException(nextToken);
                                        }
                                    }
                                    break;

                                default:
                                    ProcessGeneralContent();
                                    break;
                            }
                            break;

                        default:
                            ProcessGeneralContent();
                            break;
                    }
                    
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
