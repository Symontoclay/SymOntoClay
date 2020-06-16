using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CodeExpressionStatementParser: BaseInternalParser
    {
        public CodeExpressionStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        protected override void OnEnter()
        {
            Result = new AstExpressionStatement();
        }

        public AstExpressionStatement Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result.WriteListToString()}");
            //Log($"_state = {_state}");
#endif

            switch(_currToken.TokenKind)
            {
                case TokenKind.String:
                    ProcessStringToken();
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessStringToken()
        {
            throw new NotImplementedException();
        }

        protected override void OnFinish()
        {
            throw new NotImplementedException();
        }
    }
}
