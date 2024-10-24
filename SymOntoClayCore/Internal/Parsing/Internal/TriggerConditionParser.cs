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
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TriggerConditionParser : BaseInternalParser
    {
        public TriggerConditionParser(InternalParserContext context, params TerminationToken[] terminators)
            : base(context, terminators)
        {
        }
        
        public TriggerConditionNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private bool _hasSomething;
        private TriggerConditionNode _lastIsOperator;
        private TriggerConditionNode _lastBinaryOperator;
        private bool _hasSingleFactOrDuration;
        private bool _hasSingleEach;
        private bool _hasSingleOnce;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<TriggerConditionNode>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_currToken.TokenKind)
            {
                case TokenKind.OpenFactBracket:
                    ProcessFact();
                    break;

                case TokenKind.Word:
                    switch (_currToken.KeyWordTokenKind)
                    {
                        case KeyWordTokenKind.Unknown:
                            ProcessConceptLeaf();
                            break;

                        case KeyWordTokenKind.Duration:
                            if (_hasSingleFactOrDuration)
                            {
                                _context.Recovery(_currToken);
                                Exit();
                                break;
                            }

                            ProcessDuration();
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

                        case KeyWordTokenKind.As:
                        case KeyWordTokenKind.Alias:
                        case KeyWordTokenKind.With:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case KeyWordTokenKind.Each: 
                            ProcessEach();
                            break;

                        case KeyWordTokenKind.Once:
                            ProcessOnce();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case TokenKind.Identifier:
                    ProcessConceptLeaf();
                    break;

                case TokenKind.Var:
                case TokenKind.SystemVar:
                    ProcessVar();
                    break;

                case TokenKind.Number:
                    ProcessNumber();
                    break;

                case TokenKind.String:
                    ProcessStringToken();
                    break;

                case TokenKind.OpenRoundBracket:
                    if (_hasSingleFactOrDuration || _hasSingleEach || _hasSingleOnce)
                    {
                        _context.Recovery(_currToken);
                        Exit();
                        break;
                    }

                    ProcessRoundBrackets();
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

                case TokenKind.Point:
                    ProcessPoint();
                    break;

                case TokenKind.EntityCondition:
                    ProcessEntityCondition();
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

                case TokenKind.Lambda:
                case TokenKind.OpenFigureBracket:
                    _context.Recovery(_currToken);
                    Exit();
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessFact()
        {
            var oldHasSomething = _hasSomething;

            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var conditionNode = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Fact };
            conditionNode.RuleInstance = parser.Result;

            var intermediateNode = new IntermediateAstNode(conditionNode);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            if(!oldHasSomething)
            {
                _hasSingleFactOrDuration = true;
            }
        }

        private void ProcessEach()
        {
            var oldHasSomething = _hasSomething;

            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new TriggerConditionEachParser(_context);
            parser.Run();

            var intermediateNode = new IntermediateAstNode(parser.Result);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            if (!oldHasSomething)
            {
                _hasSingleEach = true;
            }
        }

        private void ProcessOnce()
        {
            var oldHasSomething = _hasSomething;

            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new TriggerConditionOnceParser(_context);
            parser.Run();

            var intermediateNode = new IntermediateAstNode(parser.Result);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            if (!oldHasSomething)
            {
                _hasSingleOnce = true;
            }
        }

        private void ProcessDuration()
        {
            var oldHasSomething = _hasSomething;

            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new TriggerConditionDurationParser(_context);
            parser.Run();

            var intermediateNode = new IntermediateAstNode(parser.Result);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            if (!oldHasSomething)
            {
                _hasSingleFactOrDuration = true;
            }
        }

        private void ProcessVar()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            var value = NameHelper.CreateName(_currToken.Content);

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Var };

            node.Name = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNullToken()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new NullParser(_context);
            parser.Run();

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Value };
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNumber()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);
            var parser = new NumberParser(_context);
            parser.Run();

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Value };
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessStringToken()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            var value = new StringValue(_currToken.Content);

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Value };
            node.Value = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessEntityCondition()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new ConditionalEntityParser(_context);
            parser.Run();

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Value };
            node.Value = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessConceptLeaf()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            var value = NameHelper.CreateName(_currToken.Content);

            var kindOfName = value.KindOfName;

            switch (kindOfName)
            {
                case KindOfName.Concept:
                    {
                        var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Concept };
                        node.Name = value;

                        var intermediateNode = new IntermediateAstNode(node);

                        AstNodesLinker.SetNode(intermediateNode, _nodePoint);
                    }
                    break;

                case KindOfName.Entity:
                    {
                        var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Entity };
                        node.Name = value;

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

            if(currentNode == null || currentNode.Kind == KindOfIntermediateAstNode.UnaryOperator || currentNode.Kind == KindOfIntermediateAstNode.BinaryOperator)
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
            _hasSomething = true;

            var parser = new TriggerConditionParser(_context, TokenKind.CloseRoundBracket);
            parser.Run();

            var nextToken = _context.GetToken();

            if(nextToken.TokenKind != TokenKind.CloseRoundBracket)
            {
                throw new UnexpectedTokenException(nextToken);
            }

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Group };
            node.Left = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessCallingFunction()
        {
            _lastBinaryOperator = null;
            _lastIsOperator = null;
            _hasSomething = true;

            _context.Recovery(_currToken);

            var parser = new CallingFunctionInTriggerConditionParser(_context);
            parser.Run();

            var priority = OperatorsHelper.GetPriority(KindOfOperator.CallFunction);

            var intermediateNode = new IntermediateAstNode(parser.Result, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessIsOperator()
        {
            if(!_hasSomething)
            {
                throw new UnexpectedTokenException(_currToken);
            }

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.BinaryOperator };
            node.KindOfOperator = KindOfOperator.Is;

            _lastIsOperator = node;
            _lastBinaryOperator = node;

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

            throw new UnexpectedTokenException(_currToken);
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


            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.BinaryOperator };
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

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.UnaryOperator };
            node.KindOfOperator = kindOfOperator;

            var priority = OperatorsHelper.GetPriority(kindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }
    }
}
