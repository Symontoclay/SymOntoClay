/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class EntityConditionExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            WaitForPredicateParameter,
            GotPredicateParameter,
            GotPredicate,
            GotEntity,
            GotConcept,
            GotFuzzyLogicNonNumericSequenceItem,
            GotOperator
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, bool isGroup)
            : base(context.InternalParserContext)
        {
            _entityConditionExpressionParserContext = context;
            _isGroup = isGroup;
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context)
            : this(context, false)
        {
            _entityConditionExpressionParserContext = context;
        }

        private bool _isGroup;
        private EntityConditionExpressionParserContext _entityConditionExpressionParserContext;
        private State _state = State.Init;

        public EntityConditionExpressionNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private EntityConditionExpressionNode _lastLogicalQueryNode;
        private FuzzyLogicNonNumericSequenceValue _fuzzyLogicNonNumericSequenceValue;

        private List<StrongIdentifierValue> _unresolvedAiases = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<EntityConditionExpressionNode>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.LogicalVar:
                        case TokenKind.Identifier:
                            ProcessWord();
                            break;

                        case TokenKind.Entity:
                            {
                                var name = NameHelper.CreateName(_currToken.Content);

                                var node = new EntityConditionExpressionNode();
                                node.Kind = KindOfLogicalQueryNode.Entity;
                                node.Name = name;

                                var intermediateNode = new IntermediateAstNode(node);

                                AstNodesLinker.SetNode(intermediateNode, _nodePoint);

                                _state = State.GotEntity;
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
                            {
                                _context.Recovery(_currToken);

                                var parser = new EntityConditionExpressionParser(_entityConditionExpressionParserContext);
                                parser.Run();

                                _lastLogicalQueryNode.ParamsList.Add(parser.Result);

                                _state = State.GotPredicateParameter;
                            }
                            break;

                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            {
                                switch (_currToken.KeyWordTokenKind)
                                {
                                    case KeyWordTokenKind.Null:
                                        {
                                            _context.Recovery(_currToken);

                                            var parser = new NullParser(_context);
                                            parser.Run();

                                            var node = new EntityConditionExpressionNode();
                                            node.Kind = KindOfLogicalQueryNode.Value;
                                            node.Value = parser.Result;

                                            _lastLogicalQueryNode.ParamsList.Add(node);

                                            _state = State.GotPredicateParameter;
                                        }
                                        break;

                                    default:
                                        {
                                            var nextToken = _context.GetToken();

                                            var terminatingTokenKindList = new List<TokenKind>() { TokenKind.CloseRoundBracket };

                                            if (nextToken.TokenKind != TokenKind.OpenRoundBracket)
                                            {
                                                terminatingTokenKindList.Add(TokenKind.Comma);
                                            }

                                            _context.Recovery(nextToken);
                                            _context.Recovery(_currToken);


                                            var parser = new EntityConditionExpressionParser(_entityConditionExpressionParserContext);
                                            parser.Run();

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

                                var node = new EntityConditionExpressionNode();
                                node.Kind = KindOfLogicalQueryNode.Value;
                                node.Value = parser.Result;

                                _lastLogicalQueryNode.ParamsList.Add(node);

                                _state = State.GotPredicateParameter;
                            }
                            break;

                        case TokenKind.EntityCondition:
                            {
                                _context.Recovery(_currToken);

                                var parser = new ConditionalEntityParser(_context);
                                parser.Run();


                                var node = new EntityConditionExpressionNode();
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

                        case TokenKind.Comma:
                            _state = State.WaitForPredicateParameter;
                            break;

                        case TokenKind.CloseRoundBracket:
                            Exit();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotEntity:
                case State.GotConcept:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Comma:
                        case TokenKind.CloseRoundBracket:
                            _context.Recovery(_currToken);
                            Exit();
                            break;

                        case TokenKind.And:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.And);
                            break;

                        case TokenKind.Or:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.Or);
                            break;

                        case TokenKind.Assign:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.Is);
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotOperator:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            ProcessWord();
                            break;

                        case TokenKind.OpenRoundBracket:
                            ProcessGroup();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotFuzzyLogicNonNumericSequenceItem:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
                            {
                                var value = NameHelper.CreateName(_currToken.Content);

                                _fuzzyLogicNonNumericSequenceValue.AddIdentifier(value);

                            }
                            break;

                        case TokenKind.More:
                        case TokenKind.MoreOrEqual:
                        case TokenKind.Less:
                        case TokenKind.LessOrEqual:
                            _context.Recovery(_currToken);
                            _state = State.GotConcept;
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
            var node = new EntityConditionExpressionNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.Group;

            var parser = new EntityConditionExpressionParser(_entityConditionExpressionParserContext, true);
            parser.Run();

            node.Left = parser.Result;

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotPredicate;
        }

        private void ProcessWord()
        {
            var value = NameHelper.CreateName(_currToken.Content);

#if DEBUG

#endif

            var nextToken = _context.GetToken();

#if DEBUG

#endif

            switch (value.KindOfName)
            {
                case KindOfName.Concept:
                    if (_lastLogicalQueryNode != null && _lastLogicalQueryNode.KindOfOperator == KindOfOperatorOfLogicalQueryNode.Is && _currToken.KeyWordTokenKind == KeyWordTokenKind.Not)
                    {
                        _context.Recovery(nextToken);
                        _lastLogicalQueryNode.KindOfOperator = KindOfOperatorOfLogicalQueryNode.IsNot;
                        break;
                    }

                    switch (nextToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            ProcessPredicate(value);
                            break;

                        case TokenKind.Comma:
                        case TokenKind.CloseRoundBracket:
                        case TokenKind.More:
                        case TokenKind.MoreOrEqual:
                        case TokenKind.Less:
                        case TokenKind.LessOrEqual:
                            _context.Recovery(nextToken);
                            ProcessConceptOrQuestionVar(value);
                            break;

                        case TokenKind.Word:
                            switch (nextToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    _context.Recovery(nextToken);
                                    ProcessConceptOrQuestionVar(value);
                                    break;

                                default:
                                    _context.Recovery(nextToken);
                                    StartProcessingFuzzyLogicNonNumericSequenceValue(value);
                                    break;
                            }
                            break;

                        case TokenKind.Identifier:
                            switch (_state)
                            {
                                case State.Init:
                                case State.GotOperator:
                                    _context.Recovery(nextToken);
                                    StartProcessingFuzzyLogicNonNumericSequenceValue(value);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
                            }
                            break;

                        case TokenKind.And:
                        case TokenKind.Or:
                        case TokenKind.Assign:
                            _context.Recovery(nextToken);
                            ProcessConceptOrQuestionVar(value);
                            break;

                        default:
                            throw new UnexpectedTokenException(nextToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void StartProcessingFuzzyLogicNonNumericSequenceValue(StrongIdentifierValue value)
        {
            _fuzzyLogicNonNumericSequenceValue = new FuzzyLogicNonNumericSequenceValue();

            var node = new EntityConditionExpressionNode();
            node.Kind = KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence;

            node.FuzzyLogicNonNumericSequenceValue = _fuzzyLogicNonNumericSequenceValue;

            _fuzzyLogicNonNumericSequenceValue.AddIdentifier(value);

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotFuzzyLogicNonNumericSequenceItem;
        }

        private void ProcessConceptOrQuestionVar(StrongIdentifierValue value)
        {
            var node = CreateExpressionNodeByStrongIdentifierValue(value);

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotConcept;
        }

        private EntityConditionExpressionNode CreateExpressionNodeByStrongIdentifierValue(StrongIdentifierValue value)
        {
            var node = new EntityConditionExpressionNode();

            var kindOfName = value.KindOfName;

            switch (kindOfName)
            {
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

            return node;
        }

        private void ProcessPredicate(StrongIdentifierValue name)
        {
            var node = new EntityConditionExpressionNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.Relation;

            node.Name = name;

            node.ParamsList = new List<EntityConditionExpressionNode>();

            if (_unresolvedAiases.Any())
            {
                var aliasesDict = _entityConditionExpressionParserContext.AliasesDict;

                foreach (var unresolvedAlias in _unresolvedAiases)
                {
                    if (aliasesDict.ContainsKey(unresolvedAlias))
                    {
                        throw new Exception($"Variable {unresolvedAlias.NameValue} has been bound multiple time.");
                    }

                    aliasesDict[unresolvedAlias] = node;
                }

                _unresolvedAiases.Clear();
            }

            var priority = OperatorsHelper.GetPriority(KindOfOperator.Predicate);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.UnaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.WaitForPredicateParameter;
        }

        private void ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode kindOfOperator)
        {
            var node = new EntityConditionExpressionNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.BinaryOperator;
            node.KindOfOperator = kindOfOperator;

            var kind = KindOfOperator.Unknown;

            switch (kindOfOperator)
            {
                case KindOfOperatorOfLogicalQueryNode.And:
                    kind = KindOfOperator.And;
                    break;

                case KindOfOperatorOfLogicalQueryNode.Or:
                    kind = KindOfOperator.Or;
                    break;

                case KindOfOperatorOfLogicalQueryNode.Is:
                    kind = KindOfOperator.Is;
                    break;

                case KindOfOperatorOfLogicalQueryNode.IsNot:
                    kind = KindOfOperator.IsNot;
                    break;

                case KindOfOperatorOfLogicalQueryNode.More:
                    kind = KindOfOperator.More;
                    break;

                case KindOfOperatorOfLogicalQueryNode.MoreOrEqual:
                    kind = KindOfOperator.MoreOrEqual;
                    break;

                case KindOfOperatorOfLogicalQueryNode.Less:
                    kind = KindOfOperator.Less;
                    break;

                case KindOfOperatorOfLogicalQueryNode.LessOrEqual:
                    kind = KindOfOperator.LessOrEqual;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfOperator), kindOfOperator, null);
            }

            var priority = OperatorsHelper.GetPriority(kind);

            var intermediateNode = new IntermediateAstNode(node, KindOfIntermediateAstNode.BinaryOperator, priority);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotOperator;
        }
    }
}
