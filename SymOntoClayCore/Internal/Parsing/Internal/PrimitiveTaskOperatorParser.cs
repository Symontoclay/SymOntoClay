using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using System;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PrimitiveTaskOperatorParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotOperatorMark,
            GotCallingExpr
        }

        public PrimitiveTaskOperatorParser(InternalParserContext context)
            : base(context)
        {
        }

        public PrimitiveTaskOperator Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new PrimitiveTaskOperator();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("37F9FBC3-94E5-4926-98B4-4B0C40D5A278", $"_state = {_state}");
            //Info("B3BC4040-868D-4647-B09A-25E54901EFC6", $"_currToken = {_currToken}");
            //Info(, $"Result = {Result}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Operator:
                                    _state = State.GotOperatorMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotOperatorMark:
                    {
                        _context.Recovery(_currToken);

                        var parser = new AstExpressionParser(_context, new TerminationToken(TokenKind.Semicolon, true));
                        parser.Run();

#if DEBUG
                        //Info("743FE80C-35FB-4007-A029-0E1EFBEB8DDB", $"parser.Result = {parser.Result}");
#endif

                        var statement = new AstExpressionStatement();
                        Result.Statement = statement;

                        statement.Expression = parser.Result;

                        Result.IntermediateCommandsList = _context.Compiler.CompileToIntermediateCommands(statement);

                        _state = State.GotCallingExpr;

                    }
                    break;

                case State.GotCallingExpr:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Semicolon:
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
