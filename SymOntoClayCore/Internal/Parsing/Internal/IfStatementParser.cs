/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class IfStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotIfMark,
            WaitForIfCondition,
            GotIfCondition,
            GotIfBody,
            GotElifMark,
            WaitForElifCondition,
            GotElifCondition,
            GotElifBody,
            GotElseMark,
            GotElseBody
        }

        public IfStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AstStatement Result { get; private set; }
        private AstIfStatement _rawStatement;
        private AstElifStatement _currElifStatement;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _rawStatement = new AstIfStatement();

            DefaultSettingsOfCodeEntityHelper.SetUpAnnotatedItem(_rawStatement, CurrentDefaultSetings);
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            _rawStatement.CheckDirty();

            Result = _rawStatement;

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
                                case KeyWordTokenKind.If:
                                    _state = State.GotIfMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotIfMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForIfCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForIfCondition:
                    if(IfCorrectFirstConditionToken())
                    {
                        _rawStatement.Condition = ProcessCondition();
                        _state = State.GotIfCondition;
                    }
                    else
                    {
                        throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotIfCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _rawStatement.IfStatements = ParseBody();
                            _state = State.GotIfBody;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotIfBody:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Else:
                                    _state = State.GotElseMark;
                                    break;

                                case KeyWordTokenKind.Elif:
                                    _currElifStatement = new AstElifStatement();
                                    _rawStatement.ElifStatements.Add(_currElifStatement);
                                    _state = State.GotElifMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotElifMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            _state = State.WaitForElifCondition;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForElifCondition:
                    if(IfCorrectFirstConditionToken())
                    {
                        _currElifStatement.Condition = ProcessCondition();
                        _state = State.GotElifCondition;
                    }
                    else
                    {
                        throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotElifCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _currElifStatement.Statements = ParseBody();
                            _state = State.GotElifBody;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotElifBody:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Else:
                                    _state = State.GotElseMark;
                                    break;

                                case KeyWordTokenKind.Elif:
                                    _currElifStatement = new AstElifStatement();
                                    _rawStatement.ElifStatements.Add(_currElifStatement);
                                    _state = State.GotElifMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotElseMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _rawStatement.ElseStatements = ParseBody();
                            _state = State.GotElseBody;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotElseBody:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.CloseFigureBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }
    }
}
