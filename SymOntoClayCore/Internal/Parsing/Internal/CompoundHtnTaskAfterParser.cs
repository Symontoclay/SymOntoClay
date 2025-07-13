using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class CompoundHtnTaskAfterParser: BaseCompoundHtnTaskItemsSectionParser
    {
        private enum State
        {
            Init,
            GotAfterMark,
            ContentStarted
        }

        public CompoundHtnTaskAfterParser(InternalParserContext context)
            : base(context)
        {
        }

        public CompoundHtnTaskAfter Result { get; private set; }

        private State _state = State.Init;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            Result = new CompoundHtnTaskAfter();

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
            Info("2FF58FEE-57B8-4B80-8C50-9B4ED0AC3FC1", $"_state = {_state}");
            Info("78E06C85-48AA-46F8-91C8-B43E89604DE1", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch (_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.After:
                                    _state = State.GotAfterMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(Text, _currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(Text, _currToken);
                    }
                    break;

                case State.GotAfterMark:
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
