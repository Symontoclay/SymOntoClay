using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class VarDeclParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotMark,
            GotName,
            WaitForType,
            GotType,
        }

        public VarDeclParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public VarDeclAstExpression Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new VarDeclAstExpression();
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
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Var:
                                    _state = State.GotMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Var:
                            Result.Name = NameHelper.CreateName(_currToken.Content);
                            _state = State.GotName;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            _state = State.WaitForType;
                            break;

                        case TokenKind.Semicolon:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForType:
                    Result.TypesList = ParseTypesOfParameterOrVar();

                    _state = State.GotType;
                    break;

                case State.GotType:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Assign:
                        case TokenKind.Semicolon:
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
