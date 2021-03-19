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
            GotOperator
        }

        public LogicalExpressionParser(InternalParserContext context, bool isGroup)
            : base(context)
        {
            if(isGroup)
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.CloseFactBracket };
            }
            else
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket };
            }

            _isGroup = isGroup;
        }

        public LogicalExpressionParser(InternalParserContext context)
            : this(context, new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket })
        {
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

        private bool _isGroup;
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
            //Log($"_isGroup = {_isGroup}");
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
                                var name = NameHelper.CreateName(_currToken.Content);

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
                                var name = NameHelper.CreateName(_currToken.Content);

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
                        case TokenKind.LogicalVar:
                        case TokenKind.QuestionVar:
                        {
                                _context.Recovery(_currToken);

                                var parser = new LogicalExpressionParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                _lastLogicalQueryNode.ParamsList.Add(parser.Result);

                                _state = State.GotPredicateParameter;
                            }
                            break;

                        case TokenKind.Word:
                            {
                                switch(_currToken.KeyWordTokenKind)
                                {
                                    case KeyWordTokenKind.Null:
                                        {
                                            _context.Recovery(_currToken);

                                            var parser = new NullParser(_context);
                                            parser.Run();

#if DEBUG
                                            //Log($"parser.Result = {parser.Result}");
#endif

                                            var node = new LogicalQueryNode();
                                            node.Kind = KindOfLogicalQueryNode.Value;
                                            node.Value = parser.Result;

                                            _lastLogicalQueryNode.ParamsList.Add(node);

                                            _state = State.GotPredicateParameter;
                                        }
                                        break;

                                    default:
                                        {
                                            _context.Recovery(_currToken);

                                            var parser = new LogicalExpressionParser(_context);
                                            parser.Run();

#if DEBUG
                                            //Log($"parser.Result = {parser.Result}");
#endif

                                            _lastLogicalQueryNode.ParamsList.Add(parser.Result);

                                            _state = State.GotPredicateParameter;
                                        }
                                        break;
                                }
                            }
                            break;


                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);

                                var parser = new NumberParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif
                                var node = new LogicalQueryNode();
                                node.Kind = KindOfLogicalQueryNode.Value;
                                node.Value = parser.Result;

                                _lastLogicalQueryNode.ParamsList.Add(node);

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
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.And);
                            break;

                        case TokenKind.Or:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.Or);
                            break;

                        case TokenKind.CloseRoundBracket:
                            if(_isGroup)
                            {
                                Exit();
                                break;
                            }
                            throw new UnexpectedTokenException(_currToken);

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotOperator:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            ProcessWord();
                            break;

                        case TokenKind.OpenRoundBracket:
                            ProcessGroup();
                            break;

                        case TokenKind.Not:
                            ProcessUnaryOperator(KindOfOperatorOfLogicalQueryNode.Not);
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private void ProcessGroup()
        {
            var node = new LogicalQueryNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.Group;

            var parser = new LogicalExpressionParser(_context, true);
            parser.Run();

#if DEBUG
            //Log($"parser.Result = {parser.Result}");
#endif

            node.Left = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotPredicate;
        }

        private void ProcessWord()
        {
            var value = NameHelper.CreateName(_currToken.Content);

#if DEBUG
            //Log($"value = {value}");

            //if(_currToken.Content == "NULL")
            //{
            //    throw new NotImplementedException();
            //}
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

        private void ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode kindOfOperator)
        {
            var node = new LogicalQueryNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.BinaryOperator;
            node.KindOfOperator = kindOfOperator;

            var kind = KindOfOperator.Unknown;

            switch(kindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.And:
                    kind = KindOfOperator.And;
                    break;

                case KindOfOperatorOfLogicalQueryNode.Or:
                    kind = KindOfOperator.Or;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }

            var priority = OperatorsHelper.GetPriority(kind);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotOperator;
        }

        private void ProcessUnaryOperator(KindOfOperatorOfLogicalQueryNode kindOfOperator)
        {
            var node = new LogicalQueryNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.UnaryOperator;

            node.KindOfOperator = kindOfOperator;

            var kind = KindOfOperator.Unknown;

            switch (kindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.Not:
                    kind = KindOfOperator.Not;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }

            var priority = OperatorsHelper.GetPriority(kind);

#if DEBUG
            //Log($"priority = {priority}");
#endif

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotOperator;
        }
    }
}
