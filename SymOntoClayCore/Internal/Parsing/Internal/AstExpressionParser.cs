/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AstExpressionParser : BaseInternalParser
    {
        public AstExpressionParser(InternalParserContext context, params TerminationToken[] terminators)
            : base(context, terminators)
        {
        }

        public AstExpression Result { get; private set; }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<AstExpression>();
        }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        //private bool _hasSomething;
        private BinaryOperatorAstExpression _lastIsOperator;
        private BinaryOperatorAstExpression _lastBinaryOperator;

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_currToken.TokenKind)
            {
                case TokenKind.Number:
                    ProcessNumberToken();
                    break;

                case TokenKind.String:
                    ProcessStringToken();
                    break;

                case TokenKind.Word:
                    ProcessWordToken();
                    break;

                case TokenKind.Identifier:
                case TokenKind.Entity:
                    ProcessConceptLeaf();
                    break;

                case TokenKind.Var:
                case TokenKind.SystemVar:
                    ProcessVar();
                    break;

                case TokenKind.LeftRightStream:
                    ProcessLeftRightStream();
                    break;

                case TokenKind.Plus:
                    ProcessAddition();
                    break;

                case TokenKind.Minus:
                    ProcessMinus();
                    break;

                case TokenKind.Multiplication:
                    ProcessMultiplication();
                    break;

                case TokenKind.Division:
                    ProcessDivision();
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

                case TokenKind.Assign:
                    ProcessAssign();
                    break;

                case TokenKind.Point:
                    ProcessPoint();
                    break;

                case TokenKind.Channel:
                    ProcessChannel();
                    break;

                case TokenKind.QuestionMark:
                    _context.Recovery(_currToken);
                    ProcessLogicalQueryOperator();
                    break;

                case TokenKind.EntityCondition:
                case TokenKind.OnceEntityCondition:
                    ProcessEntityCondition();
                    break;

                case TokenKind.OpenFactBracket:
                    ProcessRuleOrFact();
                    break;

                case TokenKind.OpenRoundBracket:
                    ProcessRoundBrackets();
                    break;

                case TokenKind.AsyncMarker:
                case TokenKind.DoubleAsyncMarker:
                    ProcessCallingFunction();
                    break;

                case TokenKind.Semicolon:
                    if (!_terminationTokens.Any() || _terminationTokens.Any(p => p.Equals(_currToken)))
                    {
                        Exit();
                        break;
                    }
                    throw new UnexpectedTokenException(_currToken);

                case TokenKind.OpenAnnotationBracket:
                    ProcessAnnotation();
                    break;

                case TokenKind.OpenFigureBracket:
                    ProcessFigureBracketConstruction();
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessFigureBracketConstruction()
        {
            ProcessObjectDeclaration();
        }

        private void ProcessObjectDeclaration()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new AnonymousObjectPaser(_context);
            parser.Run();

            var node = new CodeItemAstExpression() { CodeItem = parser.Result };

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessAnnotation()
        {
            _context.Recovery(_currToken);

            var parser = new AnnotationParser(_context);
            parser.Run();

            var currentAstNode = _nodePoint.CurrentNode.AstNode;

            var annotatedItem = (AnnotatedItem)currentAstNode;

            annotatedItem.AddAnnotation(parser.Result);

        }

        private void ProcessRuleOrFact()
        {
            if (_nodePoint.CurrentNode != null && _nodePoint.CurrentNode.Kind == KindOfIntermediateAstNode.Leaf)
            {
                throw new UnexpectedTokenException(_currToken);
            }

            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var ruleInstanceItem = parser.Result;

            var node = new ConstValueAstExpression();
            node.Value = ruleInstanceItem;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNumberToken()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);
            var parser = new NumberParser(_context);
            parser.Run();

            var node = new ConstValueAstExpression();
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNullToken()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new NullParser(_context);
            parser.Run();

            var node = new ConstValueAstExpression();
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessStringToken()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            var node = new ConstValueAstExpression();
            var value = new StringValue(_currToken.Content);

            node.Value = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessEntityCondition()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new ConditionalEntityParser(_context);
            parser.Run();

            var node = new ConstValueAstExpression();
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessWordToken()
        {
            switch (_currToken.KeyWordTokenKind)
            {
                case KeyWordTokenKind.Unknown:
                    ProcessConceptLeaf();
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

                case KeyWordTokenKind.Select:
                case KeyWordTokenKind.Insert:
                    {
                        var nextToken = _context.GetToken();

                        switch (nextToken.TokenKind)
                        {
                            case TokenKind.OpenFactBracket:
                                _context.Recovery(nextToken);
                                _context.Recovery(_currToken);
                                ProcessLogicalQueryOperator();
                                break;

                            default:
                                throw new UnexpectedTokenException(_currToken);
                        }
                    }
                    break;

                case KeyWordTokenKind.Null:
                    ProcessNullToken();
                    break;

                case KeyWordTokenKind.Var:
                    ProcessVarDecl();
                    break;

                case KeyWordTokenKind.Fun:
                    {
                        var predictedKeyWordTokenKind = PredictKeyWordTokenKind();

                        switch (predictedKeyWordTokenKind)
                        {
                            case KeyWordTokenKind.Unknown:
                                ProcessConceptLeaf();
                                break;

                            case KeyWordTokenKind.Fun:
                                ProcessFunDecl();
                                break;

                            default:
                                throw new UnexpectedTokenException(_currToken);
                        }
                        break;
                    }

                case KeyWordTokenKind.New:
                    {
                        var predictedKeyWordTokenKind = PredictKeyWordTokenKind();

                        switch (predictedKeyWordTokenKind)
                        {
                            case KeyWordTokenKind.Unknown:
                                ProcessConceptLeaf();
                                break;

                            case KeyWordTokenKind.New:
                                ProcessNew();
                                break;

                            default:
                                throw new UnexpectedTokenException(_currToken);
                        }
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessNew()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new NewExpressionParser(_context);
            parser.Run();

            var intermediateNode = new IntermediateAstNode(parser.Result);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessLogicalQueryOperator()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            var parser = new LogicalQueryOperationParser(_context);
            parser.Run();

            var resultItem = parser.Result;

            var node = new UnaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.CallLogicalQuery;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            var valueNode = new ConstValueAstExpression();
            valueNode.Value = resultItem;

            var valueIntermediateNode = new IntermediateAstNode(valueNode);

            AstNodesLinker.SetNode(valueIntermediateNode, _nodePoint);
        }

        private void ProcessFunDecl()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new FunctionDeclParser(_context);
            parser.Run();

            var function = parser.Result;
            function.TypeOfAccess = TypeOfAccess.Local;

            var expr = new CodeItemAstExpression() { CodeItem = function };

            var intermediateNode = new IntermediateAstNode(expr);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            if (!function.IsAnonymous)
            {
                Exit();
            }
        }

        private void ProcessVarDecl()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new VarDeclParser(_context);
            parser.Run();

            var intermediateNode = new IntermediateAstNode(parser.Result);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessVar()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            var value = NameHelper.CreateName(_currToken.Content);

            var node = new VarAstExpression();
            node.Name = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessRoundBrackets()
        {
            var currentNode = _nodePoint.CurrentNode;

            if (currentNode == null || (currentNode.Kind == KindOfIntermediateAstNode.UnaryOperator && !(currentNode.AstNode is CallingFunctionAstExpression)) || currentNode.Kind == KindOfIntermediateAstNode.BinaryOperator)
            {
                ProcessGroup();
                return;
            }

            ProcessCallingFunction();
        }

        private void ProcessGroup()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            var parser = new AstExpressionParser(_context, TokenKind.CloseRoundBracket);
            parser.Run();

            var nextToken = _context.GetToken();

            if (nextToken.TokenKind != TokenKind.CloseRoundBracket)
            {
                throw new UnexpectedTokenException(nextToken);
            }

            var groupExpression = new GroupAstExpression();
            groupExpression.Expression = parser.Result;

            var intermediateNode = new IntermediateAstNode(groupExpression);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessConceptLeaf()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            //_hasSomething = true;

            var value = NameHelper.CreateName(_currToken.Content);

            var kindOfName = value.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Concept:
                case KindOfName.Channel:
                case KindOfName.Entity:
                    {
                        var node = new ConstValueAstExpression();
                        node.Value = value;

                        var intermediateNode = new IntermediateAstNode(node);

                        AstNodesLinker.SetNode(intermediateNode, _nodePoint);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private void ProcessAssign()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Assign);
        }

        private void ProcessLeftRightStream()
        {
            ProcessUsualBinaryOperator(KindOfOperator.LeftRightStream);
        }

        private void ProcessAddition()
        {
            if (_lastBinaryOperator == null)
            {
                ProcessUsualBinaryOperator(KindOfOperator.Add);
                return;
            }

            ProcessUsualUnaryOperator(KindOfOperator.UnaryPlus);
        }

        private void ProcessMinus()
        {
            if (_lastBinaryOperator == null)
            {
                ProcessUsualBinaryOperator(KindOfOperator.Sub);
                return;
            }

            ProcessUsualUnaryOperator(KindOfOperator.UnaryMinus);
        }

        private void ProcessMultiplication()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Mul);
        }

        private void ProcessDivision()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Div);
        }

        private void ProcessPoint()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Point);
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

        private void ProcessOr()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Or);
        }

        private void ProcessAnd()
        {
            ProcessUsualBinaryOperator(KindOfOperator.And);
        }

        private void ProcessLessOrEqual()
        {
            ProcessUsualBinaryOperator(KindOfOperator.LessOrEqual);
        }

        private void ProcessUsualBinaryOperator(KindOfOperator kindOfOperator)
        {
            _lastIsOperator = null;

            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = kindOfOperator;

            _lastBinaryOperator = node;

            var priority = OperatorsHelper.GetPriority(kindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessIsOperator()
        {
            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.Is;

            _lastIsOperator = node;
            _lastBinaryOperator = node;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNotOperator()
        {
            ProcessUsualUnaryOperator(KindOfOperator.Not);
        }

        private void ProcessUsualUnaryOperator(KindOfOperator kindOfOperator)
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;

            var node = new UnaryOperatorAstExpression();
            node.KindOfOperator = kindOfOperator;

            var priority = OperatorsHelper.GetPriority(kindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

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

            throw new UnexpectedTokenException(_currToken);
        }

        private void ProcessChannel()
        {
            if (_nodePoint.CurrentNode.Kind == KindOfIntermediateAstNode.Leaf)
            {
                throw new UnexpectedTokenException(_currToken);
            }

            _lastBinaryOperator = null;
            _lastIsOperator = null;

            var name = NameHelper.CreateName(_currToken.Content);

            var node = new ConstValueAstExpression();
            node.Value = name;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessCallingFunction()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;

            _context.Recovery(_currToken);

            var parser = new CallingFunctionExpressionParser(_context);
            parser.Run();

            var node = parser.Result;

            var priority = OperatorsHelper.GetPriority(KindOfOperator.CallFunction);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }
    }
}
