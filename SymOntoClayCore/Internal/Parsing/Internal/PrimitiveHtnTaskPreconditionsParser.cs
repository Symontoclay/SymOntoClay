using SymOntoClay.Core.Internal.CodeModel;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PrimitiveHtnTaskPreconditionsParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotPreconditionsMark,
            GotConditionExpr
        }

        public PrimitiveHtnTaskPreconditionsParser(InternalParserContext context)
            : base(context)
        {
        }

        public LogicalExecutableExpression Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Info("D99532FA-6FA7-444A-B6DD-678033D29E63", $"_state = {_state}");
            Info("A4A5D717-394B-4067-8C78-4F96BC1DC9BC", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Preconditions:
                                    _state = State.GotPreconditionsMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotPreconditionsMark:
                    {
                        _context.Recovery(_currToken);

                        var parser = new AstExpressionParser(_context, new TerminationToken(TokenKind.Semicolon, true));
                        parser.Run();

                        var conditionExpr = parser.Result;

#if DEBUG
                        Info("CF50DF8B-E604-43FB-BB80-E21937EABBAE", $"conditionExpr = {conditionExpr}");
#endif

                        var compiledFunctionBody = _context.Compiler.CompileLambda(conditionExpr);

                        Result = new LogicalExecutableExpression(conditionExpr, compiledFunctionBody);

                        _state = State.GotConditionExpr;
                    }
                    break;

                case State.GotConditionExpr:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
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
