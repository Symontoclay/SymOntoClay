using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class IdleActionsParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotIdleMark,
            GotActionsMark,
            WaitForItems,
            GotItem
        }

        public IdleActionsParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public List<IdleActionItem> Result { get; private set; }
        private IdleActionItem _currentItem;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new List<IdleActionItem>();
            _currentItem = null;
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
                                case KeyWordTokenKind.Idle:
                                    _state = State.GotIdleMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotIdleMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Actions:
                                    _state = State.GotActionsMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotActionsMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.WaitForItems;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForItems:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            ProcessCodeExpression();
                            break;

                        case TokenKind.OpenFactBracket:
                            ProcessFactExpression();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            ProcessCodeExpression();
                            break;

                        case TokenKind.OpenFactBracket:
                            ProcessFactExpression();
                            break;

                        case TokenKind.CloseFigureBracket:
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

        private void ProcessCodeExpression()
        {
            _context.Recovery(_currToken);
            var parser = new CodeExpressionStatementParser(_context);
            parser.Run();

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
            //Log($"parser.Result = {parser.Result.ToHumanizedString()}");
#endif

            _currentItem = CreateIdleActionItem();
            Result.Add(_currentItem);

            _currentItem.Statements.Add(parser.Result);

#if DEBUG
            //Log($"_currentItem = {_currentItem}");
            //Log($"_currentItem = {_currentItem.ToHumanizedString()}");
#endif

            _currentItem.CompiledFunctionBody = _context.Compiler.Compile(_currentItem.Statements);

            _state = State.GotItem;
        }

        private void ProcessFactExpression()
        {
            throw new NotImplementedException();
        }
    }
}
