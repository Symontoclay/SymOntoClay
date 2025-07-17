using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CompoundHtnTaskBackgroundParser : BaseCompoundHtnTaskItemsSectionParser
    {
        private enum State
        {
            Init,
            WaitForCondition,
            GotCondition,
            ContentStarted
        }

        public CompoundHtnTaskBackgroundParser(InternalParserContext context)
            : base(context)
        {
        }

        public CompoundHtnTaskBackground Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new CompoundHtnTaskBackground();

            RegisterResult(Result);

            SetCurrentCodeItem(Result);
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            RemoveCurrentCodeEntity();
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            //Info("015E8F4C-4A36-4AE4-A9CC-6B9CBBC0DD41", $"_state = {_state}");
            //Info("03BD7953-F746-4F58-8D12-2968F007269B", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Background:
                                    _state = State.WaitForCondition;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.WaitForCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Each:
                                case KeyWordTokenKind.Once:
                                    {
                                        _context.Recovery(_currToken);

                                        var parser = new TriggerConditionParser(_context, new TerminationToken(TokenKind.OpenFigureBracket));
                                        parser.Run();

#if DEBUG
                                        //Info("B230D054-A69A-4FF0-B647-4FEE833A6CEE", $"parser.Result = {parser.Result}");
#endif

                                        Result.Condition = parser.Result;

                                        _state = State.GotCondition;
                                    }
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotCondition:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.OpenFigureBracket:
                            _state = State.ContentStarted;
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.ContentStarted:
                    ParseCompoundHtnTaskItemsSectionContent();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, $"In `{Text}`.");
            }
        }
    }
}
