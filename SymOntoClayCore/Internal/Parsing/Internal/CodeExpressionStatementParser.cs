using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
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

        /// <inheritdoc/>
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
            Log($"_nodePoint = {_nodePoint}");
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
            var node = new ConstValueAstExpression();
            var value = new StringValue(_currToken.Content);
            node.Value = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result.Expression = _nodePoint.BuildExpr<AstExpression>();
        }
    }
}
