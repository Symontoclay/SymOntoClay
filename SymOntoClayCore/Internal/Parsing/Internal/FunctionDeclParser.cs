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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class FunctionDeclParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotNamedFunctionMark,
            GotName,
            GotParameters,
            WaitForReturnType,
            GotWith,
            WaitForAction,
            GotAction
        }
        
        public FunctionDeclParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public NamedFunction Result => _namedFunction;
        private NamedFunction _namedFunction;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _namedFunction = CreateNamedFunctionAndSetAsCurrentCodeItem();

            _namedFunction.TypeOfAccess = _context.CurrentDefaultSettings.TypeOfAccess;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if(_namedFunction.Name == null)
            {
                _namedFunction.IsAnonymous = true;
                _namedFunction.Name = NameHelper.CreateEntityName();
            }

            RemoveCurrentCodeEntity();
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
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Fun:
                                    _state = State.GotNamedFunctionMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotNamedFunctionMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            var name = ParseName(_currToken.Content);
                            _namedFunction.Name = name;
                            _state = State.GotName;
                            break;

                        case TokenKind.OpenRoundBracket:
                            ProcessParameters();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            ProcessParameters();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotParameters:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.Colon:
                            _state = State.WaitForReturnType;
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.With:
                                    ProcessWith();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForReturnType:
                    {
                        _namedFunction.TypesList = ParseTypesOfParameterOrVar();

#if DEBUG
                        Info("A6533BEF-BCA8-48BD-9887-2BF517BBB5EF", $"_namedFunction.TypesList = {_namedFunction.TypesList.WriteListToString()}");
#endif

                        throw new NotImplementedException("BC650CCD-F0F8-4C73-987D-C1482E4D9860");
                    }
                    break;

                case State.GotWith:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Lambda:
                            _state = State.WaitForAction;
                            break;

                        case TokenKind.OpenFigureBracket:
                            _context.Recovery(_currToken);
                            _state = State.WaitForAction;
                            break;
                    }
                    break;

                case State.WaitForAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            {
                                _context.Recovery(_currToken);
                                var parser = new FunctionBodyParser(_context);
                                parser.Run();
                                var statementsList = parser.Result;

                                _namedFunction.Statements = statementsList;
                                _namedFunction.CompiledFunctionBody = _context.Compiler.Compile(statementsList);
                                _state = State.GotAction;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotAction:
                    _context.Recovery(_currToken);
                    Exit();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }

        private void ProcessParameters()
        {
            _context.Recovery(_currToken);

            var parser = new FunctionParametersParser(_context);
            parser.Run();

            _namedFunction.Arguments = parser.Result;

            _state = State.GotParameters;
        }

        private void ProcessWith()
        {
            _context.Recovery(_currToken);

            var parser = new WithParser(_namedFunction, _context, TokenKind.Lambda, TokenKind.OpenFigureBracket);
            parser.Run();

            _state = State.GotWith;
        }
    }
}
