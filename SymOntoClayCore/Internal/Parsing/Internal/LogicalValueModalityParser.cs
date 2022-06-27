using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class LogicalValueModalityParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotFuzzyLogicNonNumericSequenceItem
        }

        public LogicalValueModalityParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Value Result { get; private set; }

        private FuzzyLogicNonNumericSequenceValue _fuzzyLogicNonNumericSequenceValue;

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Log($"_state = {_state}");
            //Log($"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Number:
                            {
                                _context.Recovery(_currToken);
                                var parser = new NumberParser(_context, true);
                                parser.Run();

                                Result = parser.Result;

                                Exit();
                            }
                            break;

                        case TokenKind.Identifier:
                        case TokenKind.Word:
                            {
                                var kindOfRuleInstanceSectionMark = PeekKindOfRuleInstanceSectionMark();

#if DEBUG
                                //Log($"kindOfRuleInstanceSectionMark = {kindOfRuleInstanceSectionMark}");
#endif

                                if(kindOfRuleInstanceSectionMark != KindOfRuleInstanceSectionMark.Unknown)
                                {
                                    throw new UnexpectedTokenException(_currToken);
                                }

                                var nextToken = _context.GetToken();

#if DEBUG
                                //Log($"nextToken = {nextToken}");

#endif
                                switch (nextToken.TokenKind)
                                {
                                    case TokenKind.Word:
                                    case TokenKind.Identifier:
                                        _context.Recovery(nextToken);

                                        _fuzzyLogicNonNumericSequenceValue = new FuzzyLogicNonNumericSequenceValue();
                                        _fuzzyLogicNonNumericSequenceValue.AddIdentifier(ParseName(_currToken.Content));

                                        _state = State.GotFuzzyLogicNonNumericSequenceItem;
                                        break;

                                    default:
                                        _context.Recovery(nextToken);

                                        Result = ParseName(_currToken.Content);

                                        Exit();
                                        break;
                                }                                
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
                                var kindOfRuleInstanceSectionMark = PeekKindOfRuleInstanceSectionMark();

#if DEBUG
                                //Log($"kindOfRuleInstanceSectionMark = {kindOfRuleInstanceSectionMark}");
#endif

                                if(kindOfRuleInstanceSectionMark == KindOfRuleInstanceSectionMark.Unknown)
                                {
                                    _fuzzyLogicNonNumericSequenceValue.AddIdentifier(ParseName(_currToken.Content));

                                    _state = State.GotFuzzyLogicNonNumericSequenceItem;

                                    break;
                                }

                                _context.Recovery(_currToken);

#if DEBUG
                                //Log($"_fuzzyLogicNonNumericSequenceValue = {_fuzzyLogicNonNumericSequenceValue}");
#endif

                                if(_fuzzyLogicNonNumericSequenceValue.NonNumericValue != null && _fuzzyLogicNonNumericSequenceValue.Operators.IsNullOrEmpty())
                                {
                                    Result = _fuzzyLogicNonNumericSequenceValue.NonNumericValue;

                                    Exit();
                                    break;
                                }
                                

                                Result = _fuzzyLogicNonNumericSequenceValue;

                                Exit();
                            }
                            break;

                        case TokenKind.CloseFactBracket:
                            Result = _fuzzyLogicNonNumericSequenceValue;

                            _context.Recovery(_currToken);
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
    }
}
