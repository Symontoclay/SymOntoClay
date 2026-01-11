/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

                        case KeyWordTokenKind.And:
                            if (AstNodesLinker.CanBeLeafNow(_nodePoint))
                            {
                                ProcessConceptLeaf();
                            }
                            else
                            {
                                ProcessAnd();
                            }
                            break;

                        case KeyWordTokenKind.Or:
                            if (AstNodesLinker.CanBeLeafNow(_nodePoint))
                            {
                                ProcessConceptLeaf();
                            }
                            else
                            {
                                ProcessOr();
                            }
                            break;

                        case KeyWordTokenKind.Not:
                            ProcessNot();
                            break;

                        case KeyWordTokenKind.Null:
                            ProcessNullToken();
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
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
                    throw new UnexpectedTokenException(Text, _currToken);
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
                ProcessFuzzyLogicNonNumericSequence();
                return;
            }

            var value = NameHelper.CreateName(_currToken.Content);

            var kindOfName = value.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.CommonConcept:
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

        private void ProcessFuzzyLogicNonNumericSequence()
        {
            var concept = NameHelper.CreateName(_currToken.Content);

            var value = _lastConceptNode.Value;

            if(value.IsStrongIdentifierValue)
            {
                var name = value.AsStrongIdentifierValue;

                var sequence = new FuzzyLogicNonNumericSequenceValue();

                sequence.AddIdentifier(name);
                sequence.AddIdentifier(concept);

                _lastConceptNode.Value = sequence;

                return;
            }

            if(value.IsFuzzyLogicNonNumericSequenceValue)
            {
                value.AsFuzzyLogicNonNumericSequenceValue.AddIdentifier(concept);
                return;
            }

            throw new NotImplementedException("BF3E482D-EA9D-46BE-BCB8-A0CC135140E5");
        }

        private void ProcessRoundBrackets()
        {
            var currentNode = _nodePoint.CurrentNode;

            if (currentNode == null || currentNode.Kind == KindOfIntermediateAstNode.UnaryOperator || currentNode.Kind == KindOfIntermediateAstNode.BinaryOperator)
            {
                ProcessGroup();
                return;
            }

            throw new NotImplementedException("92F4AFAE-759F-4845-9294-7BCEF8E53FCF");
        }

        private void ProcessGroup()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;
            _lastConceptNode = null;

            var parser = new LogicalModalityExpressionParser(_context, TokenKind.CloseRoundBracket);
            parser.Run();

            var nextToken = _context.GetToken();

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
                throw new UnexpectedTokenException(Text, _currToken);
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

            throw new UnexpectedTokenException(Text, _currToken);
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

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }
    }
}
