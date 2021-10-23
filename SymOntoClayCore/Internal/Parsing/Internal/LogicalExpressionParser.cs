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
using System.Linq;
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
            GotFuzzyLogicNonNumericSequenceItem,
            GotOperator,
            GotAliasVar
        }

        public LogicalExpressionParser(LogicalExpressionParserContext context, bool isGroup)
            : base(context.InternalParserContext)
        {
            if(isGroup)
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.CloseFactBracket };
            }
            else
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket };
            }

            _logicalExpressionParserContext = context;
            _isGroup = isGroup;
        }

        public LogicalExpressionParser(LogicalExpressionParserContext context)
            : this(context, new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket })
        {
        }

        public LogicalExpressionParser(LogicalExpressionParserContext context, TokenKind terminatingTokenKind)
            : this(context, new List<TokenKind>() { terminatingTokenKind })
        {
        }

        public LogicalExpressionParser(LogicalExpressionParserContext context, List<TokenKind> terminatingTokenKindList)
            : base(context.InternalParserContext)
        {
            _terminatingTokenKindList = terminatingTokenKindList;
            _logicalExpressionParserContext = context;
        }

        private bool _isGroup;
        private List<TokenKind> _terminatingTokenKindList = new List<TokenKind>();
        private LogicalExpressionParserContext _logicalExpressionParserContext;
        private State _state = State.Init;

        public LogicalQueryNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private LogicalQueryNode _lastLogicalQueryNode;
        private FuzzyLogicNonNumericSequenceValue _fuzzyLogicNonNumericSequenceValue;

        private List<StrongIdentifierValue> _unresolvedAiases = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<LogicalQueryNode>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_isGroup = {_isGroup}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            //Log($"_nodePoint = {_nodePoint}");
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
                        case TokenKind.LogicalVar:
                        case TokenKind.Identifier:
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

                                var parser = new LogicalExpressionParser(_logicalExpressionParserContext);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                _lastLogicalQueryNode.ParamsList.Add(parser.Result);

                                _state = State.GotPredicateParameter;
                            }
                            break;

                        case TokenKind.Word:
                        case TokenKind.Identifier:
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
                                            var nextToken = _context.GetToken();

                                            var terminatingTokenKindList = new List<TokenKind>() { TokenKind.CloseRoundBracket };

                                            if(nextToken.TokenKind != TokenKind.OpenRoundBracket)
                                            {
                                                terminatingTokenKindList.Add(TokenKind.Comma);
                                            }

#if DEBUG
                                            //if (_currToken.Content == "distance")
                                            //{
                                                //throw new NotImplementedException();
                                            //}
#endif

#if DEBUG
                                            //Log($"nextToken = {nextToken}");
#endif

                                            _context.Recovery(_currToken);
                                            _context.Recovery(nextToken);
                                            

                                            var parser = new LogicalExpressionParser(_logicalExpressionParserContext, terminatingTokenKindList);
                                            parser.Run();

#if DEBUG
                                            //Log($"parser.Result = {parser.Result}");
                                            //if (_currToken.Content == "distance")
                                            //{
                                            //    //throw new NotImplementedException();
                                            //}
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

                        case TokenKind.EntityCondition:
                            {
                                _context.Recovery(_currToken);

                                var parser = new ConditionalEntityParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                //throw new NotImplementedException();

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

                        case TokenKind.Comma:
                            _state = State.WaitForPredicateParameter;
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

                case State.GotConcept:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Is:
                                    ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.Is);
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.More:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.More);
                            break;

                        case TokenKind.MoreOrEqual:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.MoreOrEqual);
                            break;

                        case TokenKind.Less:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.Less);
                            break;

                        case TokenKind.LessOrEqual:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.LessOrEqual);
                            break;

                        case TokenKind.And:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.And);
                            break;

                        case TokenKind.Or:
                            ProcessBinaryOperator(KindOfOperatorOfLogicalQueryNode.Or);
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

                        case TokenKind.Not:
                            ProcessUnaryOperator(KindOfOperatorOfLogicalQueryNode.Not);
                            break;

                        case TokenKind.LogicalVar:
                            {
                                var value = NameHelper.CreateName(_currToken.Content);

                                ProcessConceptOrQuestionVar(value);
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

                                var intermediateNode = new IntermediateAstNode(node);

                                AstNodesLinker.SetNode(intermediateNode, _nodePoint);

                                _state = State.GotConcept;
                            }
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

#if DEBUG
                                //Log($"value = {value}");
#endif

                                _fuzzyLogicNonNumericSequenceValue.AddIdentifier(value);

#if DEBUG
                                //Log($"_fuzzyLogicNonNumericSequenceValue = {_fuzzyLogicNonNumericSequenceValue}");
#endif
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

                case State.GotAliasVar:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                        case TokenKind.Identifier:
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

        private void ProcessGroup()
        {
            var node = new LogicalQueryNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.Group;

            var parser = new LogicalExpressionParser(_logicalExpressionParserContext, true);
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
            //Log($"value.KindOfName = {value.KindOfName}");

            //if(nextToken.Content == "is")
            //{
            //throw new NotImplementedException();
            //}
#endif

            switch (value.KindOfName)
            {
                case KindOfName.Concept:
                    if(_lastLogicalQueryNode != null && _lastLogicalQueryNode.KindOfOperator == KindOfOperatorOfLogicalQueryNode.Is && _currToken.KeyWordTokenKind == KeyWordTokenKind.Not)
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
                        case TokenKind.CloseFigureBracket:
                        case TokenKind.CloseFactBracket:
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
#if DEBUG
                                    //Log($"^)$$$$$");
#endif

                                    _context.Recovery(nextToken);
                                    StartProcessingFuzzyLogicNonNumericSequenceValue(value);
                                    break;
                            }
                            break;

                        case TokenKind.Identifier:
                            switch(_state)
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
                            _context.Recovery(nextToken);
                            ProcessConceptOrQuestionVar(value);
                            break;

                        default:
                            throw new UnexpectedTokenException(nextToken);
                    }
                    break;

                case KindOfName.LogicalVar:
                    switch(nextToken.TokenKind)
                    {
                        case TokenKind.OpenRoundBracket:
                            ProcessPredicate(value);
                            break;

                        case TokenKind.Comma:
                        case TokenKind.CloseRoundBracket:
                            _context.Recovery(nextToken);
                            ProcessConceptOrQuestionVar(value);
                            break;

                        case TokenKind.Assign:
                            _unresolvedAiases.Add(value);
                            _state = State.GotAliasVar;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new UnexpectedTokenException(_currToken);
            }
        }

        private void StartProcessingFuzzyLogicNonNumericSequenceValue(StrongIdentifierValue value)
        {
            _fuzzyLogicNonNumericSequenceValue = new FuzzyLogicNonNumericSequenceValue();

            var node = new LogicalQueryNode();
            node.Kind = KindOfLogicalQueryNode.FuzzyLogicNonNumericSequence;

            node.FuzzyLogicNonNumericSequenceValue = _fuzzyLogicNonNumericSequenceValue;

            _fuzzyLogicNonNumericSequenceValue.AddIdentifier(value);

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotFuzzyLogicNonNumericSequenceItem;
        }

        private void ProcessConceptOrQuestionVar(StrongIdentifierValue value)
        {
#if DEBUG
            //Log($"value = {value}");
#endif

            var node = CreateExpressionNodeByStrongIdentifierValue(value);

#if DEBUG
            //Log($"node = {node}");
#endif

            var intermediateNode = new IntermediateAstNode(node);

            AstNodesLinker.SetNode(intermediateNode, _nodePoint);

            _state = State.GotConcept;
        }

        private LogicalQueryNode CreateExpressionNodeByStrongIdentifierValue(StrongIdentifierValue value)
        {
            var node = new LogicalQueryNode();

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
            var node = new LogicalQueryNode();
            _lastLogicalQueryNode = node;
            node.Kind = KindOfLogicalQueryNode.Relation;

            node.Name = name;

            node.ParamsList = new List<LogicalQueryNode>();

            if(_unresolvedAiases.Any())
            {
                var aliasesDict = _logicalExpressionParserContext.AliasesDict;

                foreach (var unresolvedAlias in _unresolvedAiases)
                {
#if DEBUG
                    //Log($"unresolvedAlias = {unresolvedAlias}");
#endif

                    if(aliasesDict.ContainsKey(unresolvedAlias))
                    {
                        throw new Exception($"Variable {unresolvedAlias.NameValue} has been bound multiple time.");
                    }

                    aliasesDict[unresolvedAlias] = node;
                }

                _unresolvedAiases.Clear();
            }

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
