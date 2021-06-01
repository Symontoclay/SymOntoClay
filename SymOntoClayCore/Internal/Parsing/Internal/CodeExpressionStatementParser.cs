/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CodeExpressionStatementParser: BaseInternalParser
    {
        private enum State
        {
            Init,
            GotName,
            GotCallLogicalQueryOperator
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
            Log($"_state = {_state}");
            Log($"_currToken = {_currToken}");
            Log($"_nodePoint = {_nodePoint}");

#endif

            switch (_state)
            {
                case State.Init:
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
                            ProcessConceptLeaf();
                            break;

                        case TokenKind.Var:
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

                        case TokenKind.QuestionMark:
                            _context.Recovery(_currToken);
                            ProcessLogicalQueryOperator();
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

                        case TokenKind.Assign:
                            ProcessAssign();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotCallLogicalQueryOperator:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            ProcessRuleOrFact();
                            _state = State.Init;
                            break;

                        case TokenKind.LeftRightStream:
                            ProcessLeftRightStream();
                            _state = State.Init;
                            break;

                        case TokenKind.Semicolon:
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

        private void ProcessRuleOrFact()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var ruleInstanceItem = parser.Result;

#if DEBUG
            //Log($"ruleInstanceItem = {ruleInstanceItem}");
#endif

            var value = new RuleInstanceValue(ruleInstanceItem);
            var node = new ConstValueAstExpression();
            node.Value = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);
        }

        private void ProcessNumberToken()
        {
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
            _context.Recovery(_currToken);

            var parser = new NullParser(_context);
            parser.Run();

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
#endif

            var node = new ConstValueAstExpression();
            node.Value = parser.Result;

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
                case KeyWordTokenKind.Insert:
                    {
                        var nextToken = _context.GetToken();                        

#if DEBUG
                        //Log($"nextToken = {nextToken}");
#endif

                        switch(nextToken.TokenKind)
                        {
                            case TokenKind.OpenFactBracket:
                                _context.Recovery(_currToken);
                                _context.Recovery(nextToken);
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

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessLogicalQueryOperator()
        {
            _lastIsOperator = null;

            //_context.Recovery(_currToken);
            var parser = new LogicalQueryOperationParser(_context);
            parser.Run();

            var resultItem = parser.Result;

#if DEBUG
            //Log($"resultItem = {resultItem}");
#endif

            var node = new UnaryOperatorAstExpression();
            node.KindOfOperator = KindOfOperator.CallLogicalQuery;

            var priority = OperatorsHelper.GetPriority(node.KindOfOperator);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            var valueNode = new ConstValueAstExpression();
            valueNode.Value = resultItem;

            var valueIntermediateNode = new IntermediateAstNode(valueNode);

            AstNodesLinker.SetNode(valueIntermediateNode, _nodePoint);

            _state = State.GotCallLogicalQueryOperator;
        }

        private void ProcessVar()
        {
            _lastIsOperator = null;

            var value = NameHelper.CreateName(_currToken.Content);

            var node = new VarAstExpression();
            node.Name = value;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotName;
        }

        private void ProcessConceptLeaf()
        {
            _lastIsOperator = null;

            var value = NameHelper.CreateName(_currToken.Content);

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

        private void ProcessAssign()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Assign);
        }

        private void ProcessLeftRightStream()
        {
            ProcessUsualBinaryOperator(KindOfOperator.LeftRightStream);
        }

        private void ProcessPoint()
        {
            ProcessUsualBinaryOperator(KindOfOperator.Point);
        }

        private void ProcessUsualBinaryOperator(KindOfOperator kindOfOperator)
        {
            _lastIsOperator = null;

            var node = new BinaryOperatorAstExpression();
            node.KindOfOperator = kindOfOperator;

            var priority = OperatorsHelper.GetPriority(kindOfOperator);

#if DEBUG
            //Log($"priority = {priority}");
#endif

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

            var name = NameHelper.CreateName(_currToken.Content);

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
