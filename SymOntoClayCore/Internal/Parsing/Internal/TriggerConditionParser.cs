using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class TriggerConditionParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotDurationMark
        }

        public TriggerConditionParser(InternalParserContext context, bool closeByRoundBracket = false)
            : base(context)
        {
            _closeByRoundBracket = closeByRoundBracket;
        }

        private State _state = State.Init;

        public TriggerConditionNode Result { get; private set; }

        private IntermediateAstNodePoint _nodePoint = new IntermediateAstNodePoint();

        private bool _closeByRoundBracket;

        private bool _hasSomething;
        private TriggerConditionNode _lastIsOperator;
        private TriggerConditionNode _lastBinaryOperator;

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            if(Result == null)
            {
                Result = _nodePoint.BuildExpr<TriggerConditionNode>();
            }
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
            //Log($"Result = {Result}");            
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFactBracket:
                            Result = ParseFact();
                            Exit();
                            break;

                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Unknown:
                                    ProcessConceptLeaf();
                                    break;

                                case KeyWordTokenKind.Duration:
                                    _state = State.GotDurationMark;
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

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        case TokenKind.Var:
                            ProcessVar();
                            break;

                        case TokenKind.Number:
                            ProcessNumber();
                            break;

                        case TokenKind.CloseRoundBracket:
                            if(_closeByRoundBracket)
                            {
                                _context.Recovery(_currToken);
                                Exit();
                                break;
                            }
                            throw new UnexpectedTokenException(_currToken);

                        case TokenKind.OpenRoundBracket:
                            ProcessCallingFunction();
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotDurationMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);

                                var parser = new NumberParser(_context);
                                parser.Run();

#if DEBUG
                                //Log($"parser.Result = {parser.Result}");
#endif

                                Result = CreateDuration(parser.Result);

                                Exit();
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }

        private TriggerConditionNode ParseFact()
        {
            _context.Recovery(_currToken);

            var parser = new LogicalQueryParser(_context);
            parser.Run();

            var conditionNode = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Fact };
            conditionNode.RuleInstance = parser.Result;

            return conditionNode;
        }

        private TriggerConditionNode CreateDuration(Value value)
        {
            var conditionNode = new TriggerConditionNode() { Kind = KindOfTriggerConditionNode.Duration };
            conditionNode.Value = value;

            return conditionNode;
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
    }
}
