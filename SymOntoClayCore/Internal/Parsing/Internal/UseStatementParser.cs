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
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.RawStatements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class UseStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotUse,
            GotFirstName,
            GotIs,
            GotIsNot,
            GotSecondName,
            WaitForInheritanceRank,
            GotInheritanceRankValue,
            GotInheritanceRank
        }

        public UseStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AstStatement Result { get; private set; }
        private UseRawStatement _rawStatement;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _rawStatement = new UseRawStatement();
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
#if DEBUG
            //Log($"_rawStatement = {_rawStatement}");
#endif
            var kindOfUseRawStatement = _rawStatement.KindOfUseRawStatement;

            switch(kindOfUseRawStatement)
            {
                case KindOfUseRawStatement.UseInheritance:
                    CreateAstUseInheritanceStatement();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfUseRawStatement), kindOfUseRawStatement, null);
            }
        }

        private void CreateAstUseInheritanceStatement()
        {
            var result = new AstUseInheritanceStatement();

            DefaultSettingsOfCodeEntityHelper.SetUpAnnotatedItem(result, CurrentDefaultSetings);

            result.AppendAnnotations(_rawStatement);

            result.SubName = _rawStatement.FirstName;
            result.SuperName = _rawStatement.SecondName;

            if(_rawStatement.Rank == null)
            {
                result.Rank = new LogicalValue(1);
            }
            else
            {
                result.Rank = _rawStatement.Rank;
            }

            result.HasNot = _rawStatement.HasNot;

            Result = result;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"_rawStatement = {_rawStatement}");
            //Log($"Result = {Result}");
            //Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Use:
                                    _state = State.GotUse;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotUse:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.SystemVar:
                            _rawStatement.FirstName = ParseName(_currToken.Content);
                            _state = State.GotFirstName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFirstName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    _state = State.GotIs;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIs:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenSquareBracket:
                            _state = State.WaitForInheritanceRank;
                            break;

                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Not:
                                    _rawStatement.HasNot = true;
                                    _state = State.GotIsNot;
                                    break;

                                case KeyWordTokenKind.Unknown:
                                default:
                                    ProcessGotUseInheritanceSecondName();
                                    break;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIsNot:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenSquareBracket:
                            _state = State.WaitForInheritanceRank;
                            break;

                        case TokenKind.Word:
                            ProcessGotUseInheritanceSecondName();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSecondName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForInheritanceRank:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context, true);
                                parser.Run();
                                _rawStatement.Rank = parser.Result;
                                _state = State.GotInheritanceRankValue;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotInheritanceRankValue:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseSquareBracket:
                            _state = State.GotInheritanceRank;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotInheritanceRank:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            ProcessGotUseInheritanceSecondName();
                            break;

                        //case TokenKind.Semicolon:
                        //    Exit();
                        //    break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessGotUseInheritanceSecondName()
        {
            _rawStatement.SecondName = ParseName(_currToken.Content);
            _rawStatement.KindOfUseRawStatement = KindOfUseRawStatement.UseInheritance;
            _state = State.GotSecondName;
        }
    }
}
