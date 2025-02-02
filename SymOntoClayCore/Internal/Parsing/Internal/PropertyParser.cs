using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class PropertyParser : BaseInternalParser
    {
        private enum State
        {
            Init,
            GotPropMark
        }

        public PropertyParser(InternalParserContext context)
            : base(context)
        {
        }

        private State _state = State.Init;

        public Property Result => _property;
        private Property _property;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _property = CreateProperty();
            _property.TypeOfAccess = _context.CurrentDefaultSetings.TypeOfAccess;
        }

        /// <inheritdoc/>
        protected override void OnRun()
        {
#if DEBUG
            Info("9A4B06EC-8658-4B3B-9D65-A97DAD9C59A1", $"_state = {_state}");
            Info("584F3684-A578-4E83-AADE-38FF794F372F", $"_currToken = {_currToken}");
#endif

            switch (_state)
            {
                case State.Init:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Prop:
                                    _state = State.GotPropMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotPropMark:
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
    }
}
