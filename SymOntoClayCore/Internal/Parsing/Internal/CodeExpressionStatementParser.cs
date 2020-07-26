using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
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

                case TokenKind.Word:
                    ProcessWordToken();
                    break;

                case TokenKind.LeftRightStream:
                    ProcessLeftRightStream();
                    break;

                case TokenKind.Channel:
                    ProcessChannel();
                    break;

                case TokenKind.Semicolon:
                    Exit();
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

        private void ProcessWordToken()
        {
#if DEBUG
            Log("Begin");
#endif

            switch(_currToken.KeyWordTokenKind)
            {
                case KeyWordTokenKind.Unknown:
                    ProcessConceptLeaf();
                    break;

                case KeyWordTokenKind.Is:
                    if(AstNodesLinker.CanBeLeafNow(_nodePoint))
                    {
                        ProcessConceptLeaf();
                    }
                    else
                    {
                        ProcessIsOperator();
                    }                    
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessConceptLeaf()
        {
            var node = new ConstValueAstExpression();
            var value = NameHelper.CreateName(_currToken.Content, _context.Dictionary);
            node.Value = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessLeftRightStream()
        {
            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.LeftRightStream;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessIsOperator()
        {
            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.Is;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessChannel()
        {
            var name = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

#if DEBUG
            //Log($"name = {name}");
#endif

            var node = new ConstValueAstExpression();
            node.Value = name;

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
