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
        private enum State
        {
            Init,
            GotName,
            GotSelectLogicalQueryOperator
        }

        public CodeExpressionStatementParser(InternalParserContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new AstExpressionStatement();
        }

        private State _state = State.Init;
        public AstExpressionStatement Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private BinaryOperatorAstExpression _lastIsOperator;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"_nodePoint = {_nodePoint}");
            Log($"_state = {_state}");
#endif

            switch(_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.String:
                            ProcessStringToken();
                            break;

                        case TokenKind.Word:
                            ProcessWordToken();
                            break;

                        case TokenKind.Identifier:
                            ProcessConceptLeaf();
                            break;

                        case TokenKind.SystemVar:
                            ProcessVar();
                            break;

                        case TokenKind.LeftRightStream:
                            ProcessLeftRightStream();
                            break;

                        case TokenKind.Point:
                            ProcessPoint();
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
                    break;

                case State.GotName:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    ProcessIsOperator();
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.LeftRightStream:
                            ProcessLeftRightStream();
                            break;

                        case TokenKind.Point:
                            ProcessPoint();
                            break;

                        case TokenKind.OpenRoundBracket:
                        case TokenKind.AsyncMarker:
                            ProcessCallingFunction();
                            break;
                            
                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotSelectLogicalQueryOperator:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            ProcessRuleOrFact();
                            _state = State.Init;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessRuleOrFact()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var ruleInstanceItem = parser.Result;

#if DEBUG
            Log($"ruleInstanceItem = {ruleInstanceItem}");
#endif

            var value = new RuleInstanceValue(ruleInstanceItem);
            var node = new ConstValueAstExpression();
            node.Value = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessStringToken()
        {
            _lastIsOperator = null;

            var node = new ConstValueAstExpression();
            var value = new StringValue(_currToken.Content);

            node.Value = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessWordToken()
        {
#if DEBUG
            //Log("Begin");
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

                case KeyWordTokenKind.Not:
                    ProcessNot();
                    break;

                case KeyWordTokenKind.Select:
                    {
                        var nextToken = _context.GetToken();
                        _context.Recovery(nextToken);

#if DEBUG
                        Log($"nextToken = {nextToken}");
#endif

                        switch(nextToken.TokenKind)
                        {
                            case TokenKind.OpenFactBracket:
                                ProcessSelectOperator();
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

        private void ProcessSelectOperator()
        {
            _lastIsOperator = null;

            var node = new UnaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.SelectLogicalQuery;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

#if DEBUG
            Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotSelectLogicalQueryOperator;
        }

        private void ProcessVar()
        {
            _lastIsOperator = null;

            var value = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

            var node = new VarAstExpression();
            node.Name = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotName;
        }

        private void ProcessConceptLeaf()
        {
            _lastIsOperator = null;

            var value = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

            var kindOfName = value.KindOfName;

            switch(kindOfName)
            {
                case KindOfName.Concept:
                case KindOfName.Channel:
                case KindOfName.Entity:
                    {
                        var node = new ConstValueAstExpression();
                        node.Value = value;

                        var intermediateNode = new IntermediateAstNode(node);

                        AstNodesLinker.SetNode(intermediateNode, _nodePoint);

                        _state = State.GotName;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private void ProcessLeftRightStream()
        {
            _lastIsOperator = null;

            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.LeftRightStream;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.Init;
        }

        private void ProcessPoint()
        {
            _lastIsOperator = null;

            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.Point;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.Init;
        }

        private void ProcessIsOperator()
        {
            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.Is;

            _lastIsOperator = node;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.Init;
        }

        private void ProcessNot()
        {
#if DEBUG
            //Log($"_lastIsOperator = {_lastIsOperator}");
#endif

            if(_lastIsOperator == null)
            {
                ProcessConceptLeaf();
                return;
            }

            if(_lastIsOperator.KindOfOperator == KindOfOperator.Is)
            {
                _lastIsOperator.KindOfOperator = KindOfOperator.IsNot;
                _lastIsOperator = null;
                return;
            }
      
            throw new UnexpectedTokenException(_currToken);
        }

        private void ProcessChannel()
        {
            _lastIsOperator = null;

            var name = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

#if DEBUG
            //Log($"name = {name}");
#endif

            var node = new ConstValueAstExpression();
            node.Value = name;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessCallingFunction()
        {
            _lastIsOperator = null;

            _context.Recovery(_currToken);

            var parser = new CallingFunctionExpressionParser(_context);
            parser.Run();

            var node = parser.Result;

#if DEBUG
            //Log($"node = {node}");
#endif

            var priority = OperatorsHelper.GetPriority(KindOfOperator.CallFunction);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.Init;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result.Expression = _nodePoint.BuildExpr<AstExpression>();
        }
    }
}
