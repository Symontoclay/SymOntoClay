using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class FunctionBodyParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForStatement
        }

        public FunctionBodyParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;
        public List<AstStatement> Result { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");
            //Log($"_state = {_state}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch(_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.WaitForStatement;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForStatement:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
                            ProcessExpressionStatement();
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

        private void ProcessExpressionStatement()
        {
            _context.Recovery(_currToken);
            var parser = new CodeExpressionStatementParser(_context);
            parser.Run();
            AddStatement(parser.Result);
        }

        private void AddStatement(AstStatement statement)
        {
            if (statement != null)
            {
                Result.Add(statement);
            }
        }
    }
}
