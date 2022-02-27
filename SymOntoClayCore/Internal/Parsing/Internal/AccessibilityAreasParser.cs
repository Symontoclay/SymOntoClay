using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public class AccessibilityAreasParser: BaseObjectParser
    {
        private enum State
        {
            Init,
            GotAccessibilityAreasMark,
            ContentStarted
        }

        public AccessibilityAreasParser(InternalParserContext context, CodeItem codeItem)
            : base(context, codeItem)
        {
            _codeItem = codeItem;
        }

        private State _state = State.Init;

        private CodeItem _codeItem;

        private TypeOfAccess _prevTypeOfAccess;

        /// <inheritdoc/>
        protected override void OnEnter()
        {
            _prevTypeOfAccess = _context.CurrentDefaultSetings.TypeOfAccess;
        }

        /// <inheritdoc/>
        protected override void OnFinish()
        {
            _context.CurrentDefaultSetings.TypeOfAccess = _prevTypeOfAccess;
        }

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
                        case TokenKind.Word:
                            switch(_currToken.KeyWordTokenKind)
                            {
                                case KeyWordTokenKind.Public:
                                    _context.CurrentDefaultSetings.TypeOfAccess = TypeOfAccess.Public;
                                    _state = State.GotAccessibilityAreasMark;
                                    break;

                                case KeyWordTokenKind.Protected:
                                    _context.CurrentDefaultSetings.TypeOfAccess = TypeOfAccess.Protected;
                                    _state = State.GotAccessibilityAreasMark;
                                    break;

                                case KeyWordTokenKind.Private:
                                    _context.CurrentDefaultSetings.TypeOfAccess = TypeOfAccess.Private;
                                    _state = State.GotAccessibilityAreasMark;
                                    break;

                                default:
                                    throw new UnexpectedTokenException(_currToken);
                            }
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.GotAccessibilityAreasMark:
                    switch (_currToken.TokenKind)
                    {
                        case TokenKind.Colon:
                            _state = State.ContentStarted;
                            break;

                        default:
                            throw new UnexpectedTokenException(_currToken);
                    }
                    break;

                case State.ContentStarted:
                    ProcessGeneralContent();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_state), _state, null);
            }
        }
    }
}
