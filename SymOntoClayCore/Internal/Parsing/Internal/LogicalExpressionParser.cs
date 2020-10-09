/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForPredicateParameter,
            GotPredicateParameter,
            GotPredicate,
            GotEntity,
            GotLogicalVar,
            GotQuestionVar,
            GotConcept,
            GotBinaryOperator
        }

        public LogicalExpressionParser(InternalParserContext context, TokenKind terminatingTokenKind)
            : this(context, new List<TokenKind>() { terminatingTokenKind })
        {
        }

        public LogicalExpressionParser(InternalParserContext context, List<TokenKind> terminatingTokenKindList)
            : base(context)
        {
            _terminatingTokenKindList = terminatingTokenKindList;
        }

        private List<TokenKind> _terminatingTokenKindList = new List<TokenKind>();
        private State _state = State.Init;

        public LogicalQueryNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private LogicalQueryNode _lastLogicalQueryNode;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<LogicalQueryNode>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            //Log($"_state = {_state}");
#endif

            if(_terminatingTokenKindList.Contains(_currToken.TokenKind))
            {
                _context.Recovery(_currToken);
                Exit();
                return;
            }

            switch(_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.QuestionVar:
                            ProcessWord();
                            break;

                        case TokenKind.Entity:
                            {
                                var name = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

#if DEBUG
                                //Log($"name = {name}");
#endif

                                var node = new LogicalQueryNode();
                                node.Kind = KindOfLogicalQueryNode.Entity;
                                node.Name = name;

                                var intermediateNode = new IntermediateAstNode(node);

                                AstNodesLinker.SetNode(intermediateNode, _nodePoint);

                                _state = State.GotEntity;
                            }
                            break;

                        case TokenKind.LogicalVar:
                            {
                                var name = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

#if DEBUG
                                //Log($"name = {name}");
#endif

                                var node = new LogicalQueryNode();
                                node.Kind = KindOfLogicalQueryNode.LogicalVar;
                                node.Name = name;

                                var intermediateNode = new IntermediateAstNode(node);

                                AstNodesLinker.SetNode(intermediateNode, _nodePoint);

                                _state = State.GotLogicalVar;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.WaitForPredicateParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Entity:
                        case TokenKind.Word:
                        case TokenKind.LogicalVar:
                        case TokenKind.QuestionVar:
                        {
                                _context.Recovery(_currToken);

                                var parser = new LogicalExpressionParser(_context, new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket });
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                _lastLogicalQueryNode.ParamsList.Add(parser.Result);

                                _state = State.GotPredicateParameter;
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPredicateParameter:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.CloseRoundBracket:
                            _state = State.GotPredicate;
                            break;

                        case TokenKind.Comma:
                            _state = State.WaitForPredicateParameter;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPredicate:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.And:
                            ProcessBinaryOperator();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotBinaryOperator:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            ProcessWord();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessWord()
        {
            var value = NameHelper.CreateName(_currToken.Content, _context.Dictionary);

#if DEBUG
            //Log($"value = {value}");
#endif

            var nextToken = _context.GetToken();

#if DEBUG
            //Log($"nextToken = {nextToken}");
#endif

            switch (value.KindOfName)
            {
                case KindOfName.Concept:
                case KindOfName.QuestionVar:
                    switch(nextToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            ProcessPredicate(value);
                            break;

                        case TokenKind.Comma:
                        case TokenKind.CloseRoundBracket:
                        {
                                _context.Recovery(nextToken);

                                var node = new LogicalQueryNode();

                                var kindOfName = value.KindOfName;

                                switch (kindOfName)
                                {
                                    case KindOfName.QuestionVar:
                                        node.Kind = KindOfLogicalQueryNode.QuestionVar;
                                        node.IsQuestion = true;
                                        break;

                                    case KindOfName.Concept:
                                        node.Kind = KindOfLogicalQueryNode.Concept;
                                        break;

                                    case KindOfName.Entity:
                                        node.Kind = KindOfLogicalQueryNode.Entity;
                                        break;

                                    case KindOfName.LogicalVar:
                                        node.Kind = KindOfLogicalQueryNode.LogicalVar;
                                        break;

                                    default:
                                        throw new UnexpectedTokenException(_currToken);
                                }

                                node.Name = value;

#if DEBUG
                                //Log($"node = {node}");
#endif

                                var intermediateNode = new IntermediateAstNode(node);

                                AstNodesLinker.SetNode(intermediateNode, _nodePoint);

                                if (value.KindOfName == KindOfName.QuestionVar)
                                {
                                    _state = State.GotQuestionVar;
                                }
                                else
                                {
                                    _state = State.GotConcept;
                                }                                  
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void ProcessPredicate(StrongIdentifierValue name)
        {
            var node = new LogicalQueryNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.Relation;

            if (name.KindOfName == KindOfName.QuestionVar)
            {
                node.IsQuestion = true;
            }

            node.Name = name;

            node.ParamsList = new List<LogicalQueryNode>();

            var priority = OperatorsHelper.GetPriority(KindOfOperator.Predicate);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.WaitForPredicateParameter;
        }

        private void ProcessBinaryOperator()
        {
            var node = new LogicalQueryNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.BinaryOperator;
            node.KindOfOperator = KindOfOperatorOfLogicalQueryNode.And;

            var priority = OperatorsHelper.GetPriority(KindOfOperator.And);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotBinaryOperator;
        }
    }
}
