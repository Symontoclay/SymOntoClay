using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class WaitStatementParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotWaitMark,
            GotItem,
            WaitForItem
        }

        public WaitStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public AstStatement Result { get; private set; }
        private AstWaitStatement _rawStatement;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _rawStatement = new AstWaitStatement();

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
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Wait:
                                    _state = State.GotWaitMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                    case State.GotWaitMark:
                        switch (_currToken.TokenKind)
                        {
                            case TokenKind.Number:
                            case TokenKind.Var:
                                ProcessExpresionItem();
                                break;

                            default:
                                throw new UnexpectedTokenException(_currToken);
                        }
                        break;

                case State.GotItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                            _state = State.WaitForItem;
                            break;

                        case TokenKind.Semicolon:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                        case TokenKind.Var:
                            ProcessExpresionItem();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessExpresionItem()
        {
            _context.Recovery(_currToken);
            var parser = new AstExpressionParser(_context, TokenKind.Comma, TokenKind.Semicolon);
            parser.Run();

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
#endif

            _rawStatement.Items.Add(parser.Result);

            _state = State.GotItem;
        }
    }
}
