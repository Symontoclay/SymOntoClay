using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CompoundHtnTaskBeforeParser: BaseCompoundHtnTaskItemsSectionParser
    {
        private enum State
        {
            Init,
            GotBeforeMark,
            ContentStarted
        }

        public CompoundHtnTaskBeforeParser(InternalParserContext context)
            : base(context)
        {
        }

        public CompoundHtnTaskBefore Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new CompoundHtnTaskBefore();

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
            //Info("726578AA-AAFF-4769-890E-5DE47D7DADCA", $"_state = {_state}");
            //Info("D51E39B6-F2B8-4FD3-B05B-3C913AB3EAD9", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Before:
                                    _state = State.GotBeforeMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotBeforeMark:
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
