using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class NewExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotNewMark
        }

        public NewExpressionParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public NewAstExpression Result { get; private set; } = new NewAstExpression();

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
                                case KeyWordTokenKind.New:
                                    _state = State.GotNewMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotNewMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            {
                                var node = new ConstValueAstExpression();
                                node.Value = NameHelper.CreateName(_currToken.Content);

                                Result.Expression = node;

                                Exit();
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
