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

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<TriggerConditionNode>();
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

                        case KeyWordTokenKind.Not:
                            ProcessNot();
                            break;

                        case KeyWordTokenKind.Null:
                            ProcessNullToken();
                            break;

                        case KeyWordTokenKind.As:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case KeyWordTokenKind.Alias:
                            _context.Recovery(_currToken);
                            Exit();
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
                    if (_hasSingleFactOrDuration)
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

#if DEBUG
            //Log($"oldHasSomething = {oldHasSomething}");
#endif

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

        private void ProcessDuration()
        {
            var oldHasSomething = _hasSomething;

#if DEBUG
            //Log($"oldHasSomething = {oldHasSomething}");
#endif

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

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
#endif

            var nextToken = _context.GetToken();

#if DEBUG
            //Log($"nextToken = {nextToken}");
#endif

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

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
#endif

            var priority = OperatorsHelper.GetPriority(KindOfOperator.CallFunction);

#if DEBUG
            //Log($"priority = {priority}");
#endif

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
#if DEBUG
            //Log($"_lastBinaryOperator = {_lastBinaryOperator}");
#endif

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

            var node = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.UnaryOperator };
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
