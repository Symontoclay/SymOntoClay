using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalModalityExpressionParser : BaseInternalParser
    {
        public LogicalModalityExpressionParser(InternalParserContext context, params TerminationToken[] terminators)
            : base(context, terminators)
        {
        }

        public LogicalModalityExpressionNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private bool _hasSomething;
        private LogicalModalityExpressionNode _lastIsOperator;
        private LogicalModalityExpressionNode _lastBinaryOperator;
        private LogicalModalityExpressionNode _lastConceptNode;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<LogicalModalityExpressionNode>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_currToken.TokenKind)
            {
                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Unknown:
                            ProcessConceptLeaf();
                            break;

                        case KeyWordTokenKind.BlankIdentifier:
                            ProcessBlankIdentifier();
                            break;

                        case KeyWordTokenKind.Is:
                            if (AstNodesLinker.CanBeLeafNow(_nodePoint))
                            {
                                ProcessConceptLeaf();
                            }
                            else
                            {
                                ProcessIsOperator();
                            }
                            break;

                        case KeyWordTokenKind.Not:
                            ProcessNot();
                            break;

                        case KeyWordTokenKind.Null:
                            ProcessNullToken();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case TokenKind.Identifier:
                    ProcessConceptLeaf();
                    break;

                case TokenKind.Number:
                    ProcessNumber();
                    break;

                case TokenKind.OpenRoundBracket:
                    ProcessRoundBrackets();
                    break;

                case TokenKind.More:
                    ProcessMore();
                    break;

                case TokenKind.MoreOrEqual:
                    ProcessMoreOrEqual();
                    break;

                case TokenKind.Less:
                    ProcessLess();
                    break;

                case TokenKind.LessOrEqual:
                    ProcessLessOrEqual();
                    break;

                case TokenKind.Or:
                    ProcessOr();
                    break;

                case TokenKind.And:
                    ProcessAnd();
                    break;

                case TokenKind.Not:
                    ProcessNotOperator();
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessBlankIdentifier()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;
            _lastConceptNode = null;

            var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.BlankIdentifier };

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessConceptLeaf()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            if (_lastConceptNode != null)
            {
                throw new NotImplementedException();
            }

            var value = NameHelper.CreateName(_currToken.Content);

            var kindOfName = value.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Concept:
                    {
                        var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.Value };
                        node.Value = value;

                        _lastConceptNode = node;

                        var intermediateNode = new IntermediateAstNode(node);

                        AstNodesLinker.SetNode(intermediateNode, _nodePoint);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private void ProcessRoundBrackets()
        {
            var currentNode = _nodePoint.CurrentNode;

            if (currentNode == null || currentNode.Kind == KindOfIntermediateAstNode.UnaryOperator || currentNode.Kind == KindOfIntermediateAstNode.BinaryOperator)
            {
                ProcessGroup();
                return;
            }

            throw new NotImplementedException();
        }

        private void ProcessGroup()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;
            _lastConceptNode = null;

            var parser = new LogicalModalityExpressionParser(_context, TokenKind.CloseRoundBracket);
            parser.Run();

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
#endif

            var nextToken = _context.GetToken();

#if DEBUG
            //Log($"nextToken = {nextToken}");
#endif

            if (nextToken.TokenKind != TokenKind.CloseRoundBracket)
            {
                throw new UnexpectedTokenException(nextToken);
            }

            var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.Group };
            node.Left = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNullToken()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;
            _lastConceptNode = null;

            _context.Recovery(_currToken);

            var parser = new NullParser(_context);
            parser.Run();

            var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.Value };
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNumber()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;
            _lastConceptNode = null;

            _context.Recovery(_currToken);
            var parser = new NumberParser(_context);
            parser.Run();

            var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.Value };
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessIsOperator()
        {
            if (!_hasSomething)
            {
                throw new UnexpectedTokenException(_currToken);
            }

            var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.BinaryOperator };
            node.KindOfOperator = KindOfOperator.Is;

            _lastIsOperator = node;
            _lastBinaryOperator = node;
            _lastConceptNode = null;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNot()
        {
#if DEBUG
            //Log($"_lastIsOperator = {_lastIsOperator}");
#endif

            if (_lastIsOperator == null)
            {
                ProcessNotOperator();
                return;
            }

            if (_lastIsOperator.KindOfOperator == KindOfOperator.Is)
            {
                _lastIsOperator.KindOfOperator = KindOfOperator.IsNot;
                _lastIsOperator = null;
                return;
            }

            throw new UnexpectedTokenException(_currToken);
        }

        private void ProcessMore()
        {
            ProcessUsualBinaryOperator(KindOfOperator.More);
        }

        private void ProcessMoreOrEqual()
        {
            ProcessUsualBinaryOperator(KindOfOperator.MoreOrEqual);
        }

        private void ProcessLess()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Less);
        }

        private void ProcessLessOrEqual()
        {
            ProcessUsualBinaryOperator(KindOfOperator.LessOrEqual);
        }

        private void ProcessOr()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Or);
        }

        private void ProcessAnd()
        {
            ProcessUsualBinaryOperator(KindOfOperator.And);
        }

        private void ProcessNotOperator()
        {
            ProcessUsualUnaryOperator(KindOfOperator.Not);
        }

        private void ProcessUsualBinaryOperator(KindOfOperator kindOfOperator)
        {
            _lastIsOperator = null;
            _hasSomething = true;
            _lastConceptNode = null;

            var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.BinaryOperator };
            node.KindOfOperator = kindOfOperator;

            _lastBinaryOperator = node;

            var priority = OperatorsHelper.GetPriority(kindOfOperator);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessUsualUnaryOperator(KindOfOperator kindOfOperator)
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;
            _lastConceptNode = null;

            var node = new LogicalModalityExpressionNode() { Kind = KindOfLogicalModalityExpressionNode.UnaryOperator };
            node.KindOfOperator = kindOfOperator;

            var priority = OperatorsHelper.GetPriority(kindOfOperator);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }
    }
}
