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
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class NamedFunctionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotNamedFunctionMark,
            GotName,
            GotParameters,
            WaitForAction,
            GotAction
        }
        
        public NamedFunctionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public CodeEntity Result { get; private set; }
        private NamedFunction _namedFunction;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = CreateCodeEntity();
            Result.Kind = KindOfCodeEntity.Function;

            _namedFunction = CreateNamedFunction();
            _namedFunction.CodeEntity = Result;
            Result.NamedFunction = _namedFunction;
            Result.CodeFile = _context.CodeFile;
            Result.ParentCodeEntity = CurrentCodeEntity;
            SetCurrentCodeEntity(Result);

            if (Result.ParentCodeEntity != null)
            {
                _namedFunction.Holder = Result.ParentCodeEntity.Name;
            }
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
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
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Fun:
                                    _state = State.GotNamedFunctionMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotNamedFunctionMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            var name = ParseName(_currToken.Content);
                            Result.Name = name;
                            _namedFunction.Name = name;
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            {
                                _context.Recovery(_currToken);

                                var parser = new FunctionParametersParser(_context);
                                parser.Run();

                                _namedFunction.Arguments = parser.Result;

                                _state = State.GotParameters;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
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

                        default:
                            throw new UnexpectedTokenException(_currToken);
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
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAction:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            _context.Recovery(_currToken);
                            Exit();
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