using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class EntityConditionExpressionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotEntity,
            GotConcept
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, bool isGroup)
            : base(context.InternalParserContext)
        {
            if (isGroup)
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.CloseRoundBracket };
            }
            else
            {
                _terminatingTokenKindList = new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket };
            }

            _logicalExpressionParserContext = context;
            _isGroup = isGroup;
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context)
            : this(context, new List<TokenKind> { TokenKind.Comma, TokenKind.CloseRoundBracket })
        {
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, TokenKind terminatingTokenKind)
            : this(context, new List<TokenKind>() { terminatingTokenKind })
        {
        }

        public EntityConditionExpressionParser(EntityConditionExpressionParserContext context, List<TokenKind> terminatingTokenKindList)
            : base(context.InternalParserContext)
        {
            _terminatingTokenKindList = terminatingTokenKindList;
            _logicalExpressionParserContext = context;
        }

        private bool _isGroup;
        private List<TokenKind> _terminatingTokenKindList = new List<TokenKind>();
        private EntityConditionExpressionParserContext _logicalExpressionParserContext;
        private State _state = State.Init;

        public EntityConditionExpressionNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private EntityConditionExpressionNode _lastLogicalQueryNode;
        private FuzzyLogicNonNumericSequenceValue _fuzzyLogicNonNumericSequenceValue;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            Result = _nodePoint.BuildExpr<EntityConditionExpressionNode>();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");
            Log($"_state = {_state}");
#endif

            if (_terminatingTokenKindList.Contains(_currToken.TokenKind))
            {
                _context.Recovery(_currToken);
                Exit();
                return;
            }

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

#if DEBUG
                                //Log($"name = {name}");
#endif

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

                case State.GotEntity:
                    switch (_currToken.TokenKind)
                    {
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
            var value = NameHelper.CreateName(_currToken.Content);

#if DEBUG
            Log($"value = {value}");

            //if(_currToken.Content == "NULL")
            //{
            //    throw new NotImplementedException();
            //}
#endif

            var nextToken = _context.GetToken();

#if DEBUG
            Log($"nextToken = {nextToken}");
            Log($"value.KindOfName = {value.KindOfName}");

            //if(nextToken.Content == "is")
            //{
            //throw new NotImplementedException();
            //}
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
                        case TokenKind.Comma:
                        case TokenKind.CloseRoundBracket:
                        case TokenKind.More:
                        case TokenKind.MoreOrEqual:
                        case TokenKind.Less:
                        case TokenKind.LessOrEqual:
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
    }
}
